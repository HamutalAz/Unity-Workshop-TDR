const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const InGamePlayersAmount = 5;
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
              createNewGame(arr);
              //rooms allocation algorithm
              deleteFromLobby(arr);
              
          });
      }
      console.log("Not enough players in lobby to start a new game!");
      return null;
      
    });

/** creating a new game collection & adding users
 * @param {arr} arr - an array of dictionarys containing users data.
 * @return {promise}
 */
function createNewGame(arr) {
  console.log("***** creating game! *****");
  const newGameRef = db.collection("Games").doc();
  const newGameMembersRef = newGameRef.collection("game_members");
  return arr.forEach(async (val) => {
    //creates a document for each player
    newGameMembersRef.doc(val.id).set({
      userName: val.userName,
    });
    //updates the gameId under Users/{user-id}
     db.collection("Users").doc(val.id).update({
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
