const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const InGamePlayersAmount = 10;
const lobbyPath = "Lobby/gYdtPMVaorwoc2jH3Iog/room_members/{user_id}";
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

// listen on player's joinning the game & move then to game
exports.checkNumberOfPlayersInLobby = firestore
    .document(lobbyPath).onCreate((snap, context)=>{
      console.log("***** checkNumberOfPlayersInLobby: invoked! *****");
      // get all players from the lobby & check how many player in there
      const lobbyCol = db.collection("Lobby");
      const lobbyDoc = lobbyCol.doc("gYdtPMVaorwoc2jH3Iog");
      return lobbyDoc.collection("room_members").get()
          .then((snapshot) => {
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
            // if enough players in lobby: create a new game & delete
            // the players from the lobby
            if (size >= InGamePlayersAmount) {
              /* console.log("size >= " + InGamePlayersAmount +
              ", creating new game!"); */
              createNewGame(arr);
              deleteFromLobby(arr);
            }
          });
    });

/** creating a new game collection & adding users
 * @param {arr} arr - an array of dictionarys containing users data.
 * @return {promise}
 */
function createNewGame(arr) {
  console.log("***** creating game! *****");
  const newGameRef = db.collection("Games").doc();
  const newGameMembersRef = newGameRef.collection("room_members");
  return arr.forEach((val) => {
    newGameMembersRef.doc(val.id).set({
      userName: val.userName,
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
  lobbyCol = lobbyDoc.collection("room_members");
  return arr.forEach((val) => {
    lobbyCol.doc(val.id).delete().then(() => {
      // console.log("Document " + val.id + " successfully deleted!");
    });
  });
}
