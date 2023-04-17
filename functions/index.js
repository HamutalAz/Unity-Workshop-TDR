const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const lobbyPath = "Lobby/gYdtPMVaorwoc2jH3Iog/lobby_members/{user_id}";
const admin = require("firebase-admin");
let currentGameId = null;
admin.initializeApp();
const db = admin.firestore();
let maxPlayers = 0;

// listen on player's joinning the game & move then to game
exports.checkNumberOfPlayersInLobby = firestore
    .document(lobbyPath).onCreate(async(snap, context)=>{
      console.log("***** checkNumberOfPlayersInLobby: invoked! *****");
      
      // get all players from the lobby & check how many player in there
      const lobbyCol = db.collection("Lobby");
      const lobbyDoc = lobbyCol.doc("gYdtPMVaorwoc2jH3Iog");
      const room_mem = lobbyDoc.collection("lobby_members");
      const snapshot = await room_mem.count().get();
      const numOfPlayers = snapshot.data().count;
      maxPlayers = (await lobbyDoc.get()).data().MaxPlayers;
      console.log("MaxPlayers is: " + maxPlayers);
      
      //check if there are enough players in lobby
      if(numOfPlayers >=maxPlayers){
        return room_mem.get().then((snapshot) => {
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
              createNewGame(arr,numOfPlayers);
              deleteFromLobby(arr);
              let levelsOrder = shuffleLevels();
              matchMaking(arr, numOfPlayers, currentGameId, levelsOrder);
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
  const newGameMembersRef = newGameRef.collection("game_members");

  return arr.forEach(async (val) => {
      
      //creates a document for each player
      await newGameMembersRef.doc(val.id).set({
        userName: val.userName,
      });
      
      //updates the gameId under Users/{user-id}
      await db.collection("Users").doc(val.id).update({
        gameId: newGameRef.id
      });
    });
}

/** deleting users from the lobby
 * @param {arr} arr - an array of dictionarys containing users data.
 * @return {promise}
 */
function deleteFromLobby(arr) {
  console.log("***** deleting from lobby! *****");
  
  let lobbyCol = db.collection("Lobby");
  const lobbyDoc = lobbyCol.doc("gYdtPMVaorwoc2jH3Iog");
  lobbyCol = lobbyDoc.collection("lobby_members");
  
  return arr.forEach((val) => {
    lobbyCol.doc(val.id).delete().then(() => {
      // console.log("Document " + val.id + " successfully deleted!");
    });
  });
}

/** creating rooms for players
 * @param {arr, numOfPlayers} - an array of dictionarys containing users data, number of users.
 * @return {promise}
 */

async function matchMaking(arr, numOfPlayers, gameId, levelsOrder){
  console.log("***** matchMaking! *****");

  let isFinal = true;
  let numberOfPlayersInRoom = 1;
  if(numOfPlayers > 3){
      isFinal = false;
      numberOfPlayersInRoom = setNumberOfPlayersInRoom(numOfPlayers);
  }
  const numberOfRoomsToBeEliminated = Math.floor(numOfPlayers/numberOfPlayersInRoom/2);
  const numberOfRoomsToQualify = numOfPlayers/numberOfPlayersInRoom - numberOfRoomsToBeEliminated;
  
  /*update all game info needed for game => number of players, players in room, players to be eliminated 
  during this stage of the game, number of rooms, is final or not, etc...
  */
  await db.collection("Games").doc(gameId).set({
    numberOfPlayers: numOfPlayers,
    numberOfPlayersInARoom: numberOfPlayersInRoom,
    numberOfRooms: numOfPlayers/numberOfPlayersInRoom,
    numberOfRoomsToQualify: numberOfRoomsToQualify,
    numberOfRoomsToEliminate: numberOfRoomsToBeEliminated,
    isFinal: isFinal,
    levelsOrder: levelsOrder,
    currentLevelInd: 0
  });
      
  const shuffledArr = shuffle(arr);
  for(i=0;i<numOfPlayers/numberOfPlayersInRoom;i++){
    
    //creates a document for the room at Rooms/{roomId}.
    const roomRef = db.collection("Rooms").doc();
    
    //creates a document for the room at Games/{gameId}/game_rooms/{roomId}
    await db.collection("Games").doc(gameId).collection("game_rooms")
    .doc(roomRef.id).set({
     roomId: roomRef.id,
    });
    
    //updates the gameId under Rooms/{roomId}
    await roomRef.set({
      gameId: gameId
    });
    for(j=0;j<numberOfPlayersInRoom;j++){
      const user = shuffledArr[numberOfPlayersInRoom*i + j];
      
      //creates a document for the player at Rooms/{roomId}/room_members/{userId}
      await roomRef.collection("room_members")
      .doc(user.id).set({
        userName: user.userName,
      });
      
      /*updates roomId at Users/{userId}
      this code should be the last thing running for each matchmaking, beacaues it will trigger
      an event of room loading at front-end, so every info about the room should already be ready.
      */
      await db.collection("Users").doc(user.id).update({
        roomId: roomRef.id
      });

    }
  }
}

function setNumberOfPlayersInRoom(numOfPlayers){
  
  if(numOfPlayers <= 3){
    return 1;
  }
  const choices = [1];
  
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
  // const levelsCol = db.collection("Levels");
  // const snapshot = await levelsCol.count().get();
  // const n = await snapshot.data().count;
  const n = 5; // change to amount of levels we create
  let levelsOrder = [];

  // initialize array
  for (let i = 0; i < n; i++) {
      levelsOrder[i] = "level" + Number(i + 1);
  }
  return shuffleArray(levelsOrder); // shuffle the array
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

// rtyingggggg
exports.addMessage = functions.https.onRequest(async (req, res) => {
  setPlayersLocationInRooms("level1");
  res.json({result: `Sent.`});
});


async function setPlayersLocationInRooms(currentLevel){
  const gameMembersCol = db.collection("Games").doc("rZ15WJTyvvdZiUEgeMfg").collection("game_members");

  // const gameMembersCol = db.collection("Games").doc(currentGameId).collection("game_members");
  let usersIDs = [];

  await gameMembersCol.get().then((snapshot) => {
    snapshot.forEach((doc) => {
      usersIDs.push(doc.id);
    });
  });

  const levelDoc = db.collection("Levels").doc(currentLevel);
  let data = (await levelDoc.get()).data();
  let xArr = data.roomBordersX;
  let y = data.playerYVal;
  let zArr = data.roomBordersZ;
  let loc = null;

  return usersIDs.forEach((id) => {
    // locatePlayers(id);
    loc = generateRandomLoc(xArr, y, zArr);
    db.collection("Users").doc(id).update({
      location: loc
    });
  })
}

// const locatePlayers = async (id) => {
//   let loc = generateRandomLoc(xArr, y, zArr);

//   functions.logger.log('Updating loc', loc)

//   try {
//     const usersRef = db.collection("Users").doc(id);
//     await usersRef.update({
//       location: loc
//     });
//     functions.logger.log('Success user updated!', loc)
//   } catch (error) {
//     functions.logger.error('Error updating user!', error)
//   }
// }

function generateRandomLoc(xArr, y, zArr){
  console.log("***** generateRandomLoc *****");
  const x = randomFromInterval(xArr[0], xArr[1]);
  const z = randomFromInterval(zArr[0], zArr[1]);

  let loc = "(" + x.toFixed(2) + ", " + y + ", " + z.toFixed(2) + ")";
  console.log("Location choosen: " + loc);
  return loc;
}

function randomFromInterval(min, max) {
  return Math.random() * (max - min + 1) + min
}


