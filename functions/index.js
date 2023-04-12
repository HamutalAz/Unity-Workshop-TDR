const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const InGamePlayersAmount = 6;
const lobbyPath = "Lobby/gYdtPMVaorwoc2jH3Iog/lobby_members/{user_id}";
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

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
      //check if there are enough players in lobby
      if(numOfPlayers >=InGamePlayersAmount){
        return room_mem.get().then((snapshot) => {
            let size = 0;
            const arr = [];
            // console.log("**** getting snapshot data! ****");
            snapshot.forEach((doc) => {
              // console.log(doc.id, " => ", doc.data());
              const dict = {
                "id": doc.id,
                "userName": doc.data().userName,
              };
              arr.push(dict);
              size = size + 1;
            });
            console.log("there are " + size + " players in lobby");
              createNewGame(arr,numOfPlayers);
              //matchMaking(arr,numOfPlayers, null);
              //rooms allocation algorithm
              //deleteFromLobby(arr);
              
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
  const newGameMembersRef = newGameRef.collection("game_members");

  arr.forEach(async (val) => {
      //creates a document for each player
      await newGameMembersRef.doc(val.id).set({
        userName: val.userName,
      });
      //updates the gameId under Users/{user-id}
      await db.collection("Users").doc(val.id).update({
        gameId: newGameRef.id
      });
    });

  return matchMaking(arr,numOfPlayers,newGameRef.id);
  // return arr.forEach(async (val) => {
  //   //creates a document for each player
  //   await newGameMembersRef.doc(val.id).set({
  //     userName: val.userName,
  //   });
  //   //updates the gameId under Users/{user-id}
  //   await db.collection("Users").doc(val.id).update({
  //     gameId: newGameRef.id
  //   });
  // });
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

async function matchMaking(arr, numOfPlayers, gameId){
  console.log("MatchMaking!!");
  let isFinal = true;
  let numberOfPlayersInRoom = 1;
  if(numOfPlayers > 3){
      isFinal = false;
      numberOfPlayersInRoom = setNumberOfPlayersInRoom(numOfPlayers);
  }
  const array = getShuffledArray(numOfPlayers);

  for(i=0;i<numOfPlayers/numberOfPlayersInRoom;i++){
    //creates a document for the room at Rooms/{roomId}.
    const roomRef = db.collection("Rooms").doc();
    //creates a document for the room at Games/{gameId}/game_rooms/{roomId}

    // await db.collection("Games").doc(gameId).collection("game_rooms")
    //  .doc(roomRef.id).set();
    
    //updates the gameId under Rooms/{roomId}
    roomRef.set({
      gameId: gameId
    })
    for(j=0;j<numberOfPlayersInRoom;j++){
      const user = arr[numberOfPlayersInRoom*i + j];
      //creates a document for the player at Rooms/{roomId}/room_members/{userId}
      await roomRef.collection("room_members")
      .doc(user.id).set({
        userName: user.userName,
      });
      //updates roomId at Users/{userId}
    }


    //create room document in Rooms/{roomId} => done
    //update room in Games/{gameID} /game_rooms/{roomId} => done
    //random players to rooms => done
    //update room in Users/{userId}/roomID => not done!
    
    //update game info => numofplayers, num in room, num to eliminate, room name etc.
    
  }
}

function setNumberOfPlayersInRoom(numOfPlayers){
  //**final**
  if(numOfPlayers <= 3){
    return 1;
  }
  const choices = [1];
  if(numOfPlayers % 2 == 0)
      choices.push(2);
  if(numOfPlayers % 3 == 0)
      choices.push(3);
  return choices[getRandomInt(choices.length)];
}

function getRandomInt(max) {
  return Math.floor(Math.random() * max);
}

function getShuffledArray(numOfPlayers){
  arr = [];
  for(i=0;i<numOfPlayers;i++){
    arr.push(i);
  }
  return shuffle(arr);
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
