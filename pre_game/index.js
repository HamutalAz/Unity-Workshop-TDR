const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

const lobbyPath = "Lobby/x3XGhSKLrwiXtcgyBuIr/lobby_members/{user_id}";
let currentGameId = null;
let maxPlayers = 0;

// listen on player's joinning the game & move then to game
exports.checkNumberOfPlayersInLobby = firestore
    .document(lobbyPath).onCreate(async(snap, context)=>{
      console.log("***** checkNumberOfPlayersInLobby: invoked! *****");
      
      // get all players from the lobby & check how many player in there
      const lobbyCol = db.collection("Lobby");
      const lobbyDoc = lobbyCol.doc("x3XGhSKLrwiXtcgyBuIr");
      const room_mem = lobbyDoc.collection("lobby_members");
      const snapshot = await room_mem.count().get();
      const numOfPlayers = snapshot.data().count;
      maxPlayers = (await lobbyDoc.get()).data().MaxPlayers;
      
      //check if there are enough players in lobby
      if(numOfPlayers >=maxPlayers){
        return room_mem.get().then(async(snapshot) => {
            let size = 0;
            const arr = [];
            snapshot.forEach((doc) => {
              const dict = {
                "id": doc.id,
                "userName": doc.data().userName,
              };
              arr.push(dict);
              size = size + 1;
            });
            console.log("there are " + size + " / " + maxPlayers +" players in lobby");
              await createNewGame(arr,numOfPlayers);
              await deleteFromLobby(arr);
              let levelsOrder = shuffleLevels();
              return matchMaking(arr, numOfPlayers, currentGameId, levelsOrder);
          });
      }
      console.log("Not enough players in lobby to start a new game!");
      return null;
      
    });

/** creating a new game collection & adding users
 * @param {arr} arr - an array of dictionarys containing users data.
 * @return {promise}
 */
function createNewGame(arr,numOfPlayers) {
  console.log("***** creating game! *****");
  
  const newGameRef = db.collection("Games").doc();
  currentGameId = newGameRef.id;
  const promises = [];
  const values = newGameRef.set({
    currentLevelInd: 0,
    numberOfPlayers: numOfPlayers
  });
  promises.push(values);
  const newGameMembersRef = newGameRef.collection("game_members");
  arr.forEach(async (val) => {
    console.log("currently in createNewGame with player: " + val);
    const p = newGameMembersRef.doc(val.id).set({
      userName: val.userName,
    });
    promises.push(p);
    const q = db.collection("Users").doc(val.id).update({
      gameId: newGameRef.id
    });
    promises.push(q);
    });
  return Promise.all(promises);
}

/** deleting users from the lobby
 * @param {arr} arr - an array of dictionarys containing users data.
 * @return {promise}
 */
function deleteFromLobby(arr) {
  console.log("***** deleting from lobby! *****");
  
  let lobbyCol = db.collection("Lobby");
  const lobbyDoc = lobbyCol.doc("x3XGhSKLrwiXtcgyBuIr");
  lobbyCol = lobbyDoc.collection("lobby_members");
  const promises = [];
  arr.forEach((val) => {
    const p = lobbyCol.doc(val.id).delete();
    promises.push(p);
  });
  return Promise.all(promises);
}

/** creating rooms for players
 * @param {arr, numOfPlayers} - an array of dictionarys containing users data, number of users.
 * @return {promise}
 */
async function matchMaking(arr, numOfPlayers, gameId, levelsOrder){
  console.log("***** matchMaking! *****");

  let isFinal = true;
  let numberOfPlayersInRoom = 2; //todo: change to 1
  let currentGameInfo = await db.collection("Games").doc(gameId).get();
  let currentLevel = levelsOrder[currentGameInfo.data().currentLevelInd];
  if(numOfPlayers > 4){
      isFinal = false;
      
      numberOfPlayersInRoom = setNumberOfPlayersInRoom(numOfPlayers,currentLevel);
  }
  const numberOfRoomsToBeEliminated = Math.floor(numOfPlayers/numberOfPlayersInRoom/2);
  const numberOfRoomsToQualify = numOfPlayers/numberOfPlayersInRoom - numberOfRoomsToBeEliminated;
  
  /*update all game info needed for game => number of players, players in room, players to be eliminated 
  during this stage of the game, number of rooms, is final or not, etc...
  */
  await db.collection("Games").doc(gameId).update({
    numberOfPlayersInARoom: numberOfPlayersInRoom,
    numberOfRooms: numOfPlayers/numberOfPlayersInRoom,
    numberOfRoomsToQualify: numberOfRoomsToQualify,
    numberOfRoomsToEliminate: numberOfRoomsToBeEliminated,
    Qualified: 0,
    isFinal: isFinal,
    levelsOrder: levelsOrder,
  });
  
  const promises = [];
  const shuffledArr = shuffle(arr);
  for(i=0;i<numOfPlayers/numberOfPlayersInRoom;i++){
    
    //creates a document for the room at Rooms/{roomId}.
    const roomRef = db.collection("Rooms").doc();
    
    //creates a document for the room at Games/{gameId}/game_rooms/{roomId}
    const a = db.collection("Games").doc(gameId).collection("game_rooms")
    .doc(roomRef.id).set({
     roomId: roomRef.id,
    });
    promises.push(a);
    
    //updates the gameId under Rooms/{roomId}
    const b = roomRef.set({
      gameId: gameId,
      status: "mid-game"
    });
    promises.push(b);

    //copy all level objects to room!
    promises.push(copyAllLevelObjects(levelsOrder[0], roomRef)); // todo: make it not hard codded!!!!
    //promises.push(copyAllLevelObjects("level1" ,roomRef));

    for(j=0;j<numberOfPlayersInRoom;j++){
      const user = shuffledArr[numberOfPlayersInRoom*i + j];
      
      //creates a document for the player at Rooms/{roomId}/room_members/{userId}
      const c = roomRef.collection("room_members")
      .doc(user.id).set({
        userName: user.userName,
      });
      promises.push(c);

      /*updates roomId at Users/{userId}
      this code should be the last thing running for each matchmaking, beacaues it will trigger
      an event of room loading at front-end, so every info about the room should already be ready.
      */
      const d = db.collection("Users").doc(user.id).update({
        roomId: roomRef.id
      });
      promises.push(d);

    }
  }
  await Promise.all(promises);
  console.log("At MatchMaking: finished creating rooms, and updating game and users!");
  
  await setPlayersLocationInRooms(currentLevel); //todo: change to levelsOrder[0]
  console.log("At MatchMaking: finished players location creation!");

  //update about game: ready to load the game scene, all data is ready!
  return db.collection("Games").doc(currentGameId).collection("game_status")
  .doc().set({
    readyToLoad : true,
  });

  //async functions must return a promise!!!!!
}


async function setPlayersLocationInRooms(currentLevel){
  // const gameMembersCol = db.collection("Games").doc("rZ15WJTyvvdZiUEgeMfg").collection("game_members");

  const gameMembersCol = db.collection("Games").doc(currentGameId).collection("game_members");
  let usersIDs = [];
  await gameMembersCol.get().then((snapshot) => {
    snapshot.forEach((doc) => {
      usersIDs.push(doc.id);
    });
  });

  console.log("currentLevel" + currentLevel);
  const levelDoc = db.collection("Levels").doc(currentLevel);
  console.log("levelDoc" + levelDoc);
  let data = (await levelDoc.get()).data();
  let subRoom = data.subRooms;
  let xArr = data.roomBordersX;
  let y = data.playerYVal;
  let zArr = data.roomBordersZ;
  let xArr2 = null;
  let zArr2 = null;
  if(subRoom){
    xArr2 = data.roomBordersX2;
    zArr2 = data.roomBordersZ2;
  }
  let loc = null;
  //console.log("xarr, y, zarr: " + xArr + y +zArr);
  promises = []
  for (var i = 0; i < usersIDs.length; i++) {
    if(subRoom){
      if(i%2 == 0){
        loc = generateRandomLoc(xArr, y, zArr);
      } else{
        loc = generateRandomLoc(xArr2, y, zArr2);
      }
    } else{
      loc = generateRandomLoc(xArr, y, zArr);
    }
    const p = db.collection("Users").doc(usersIDs[i]).update({
      location: loc
    });
    promises.push(p);
  }
  return Promise.all(promises);  
  // usersIDs.forEach(async(id) => {
  //   loc = generateRandomLoc(xArr, y, zArr);
  //   const p = db.collection("Users").doc(id).update({
  //     location: loc
  //   });
  //   promises.push(p);
  // });
  
}

async function copyAllLevelObjects(currentLevel, roomRef){
  const levelObjectsCollection = db.collection("Levels").doc(currentLevel).collection("room_objects");
  const roomObjectsCollection = roomRef.collection("room_objects");

  const snapshot = await levelObjectsCollection.get();
  promises = [];
  snapshot.docs.forEach(doc => {
    const p = roomObjectsCollection.doc(doc.id).set(doc.data());
    promises.push(p);
  });
  return Promise.all(promises);
}




//All this functions do not contact with database, they are logic functions!~
function setNumberOfPlayersInRoom(numOfPlayers, currentLevel){
  
  if(numOfPlayers <= 3){
    return 1;
  }
  
  const choices = [];
  if(currentLevel != "level2"){
    choices.push(1);
  }
  if(numOfPlayers % 2 == 0)
      choices.push(2);
  
  if(numOfPlayers % 3 == 0)
      choices.push(3);
  
  console.log("options for number of players in room: " + choices);
  const result = choices[getRandomInt(choices.length)];
  
  console.log("chosen number players in room: " + result);
  return result;
}

function getRandomInt(max) {
  return Math.floor(Math.random() * max);
}


function shuffle(array) {
  let currentIndex = array.length,  randomIndex;

  // While there remain elements to shuffle.
  while (currentIndex != 0) {

    // Pick a remaining element.
    randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex--;

    // And swap it with the current element.
    [array[currentIndex], array[randomIndex]] = [
      array[randomIndex], array[currentIndex]];
  }
  return array;
}

/** randomly choose an order for the levels to be played && store it in the DB
 * @return {levelsOrder} - the order of the levels in which we'll play the game
 */
function shuffleLevels() {
  const n = 2; // change to amount of levels we create
  let level2 = [];
  level2[0] = "level2";
  let levelsOrder = ["level1"];
  return level2.concat(shuffleArray(levelsOrder));
  // initialize array // todo: uncomment below line & delete following line
  // for (let i = 0; i < 2; i++) {
  //     levelsOrder[i] = "level" + Number(i + 1);
  // }
  // levelsOrder[0] = "level2";
  //return shuffleArray(levelsOrder); // shuffle the array

  // return levelsOrder;
}


function shuffleArray(arr) {
  for (let i = arr.length - 1; i >= 1; i--) {
      let j = Math.floor(Math.random() * (i + 1));
      let temp = arr[i];
      arr[i] = arr[j];
      arr[j] = temp;
  }
  return arr;
}

function generateRandomLoc(xArr, y, zArr){
  console.log("***** generateRandomLoc *****");
  console.log(xArr,zArr);
  const x = randomFromInterval(xArr[0], xArr[1]);
  const z = randomFromInterval(zArr[0], zArr[1]);

  let loc = "(" + x.toFixed(2) + ", " + y + ", " + z.toFixed(2) + ")";
  console.log("Location choosen: " + loc);
  return loc;
}

function randomFromInterval(min, max) {
  return Math.random() * (max - min + 1) + min
}

