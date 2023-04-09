const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const InGamePlayersAmount = 10;
const lobbyPath = "Lobby/gYdtPMVaorwoc2jH3Iog/room_members/{user_id}";
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

// listen on player's joinning the game
exports.checkNumberOfPlayersInLobby = firestore
    .document(lobbyPath).onCreate((snap, context)=>{
      console.log("***** checkNumberOfPlayersInLobby: invoked! *****");
      // get all players from the lobby & check how many player in there
      const lobbyCol = db.collection("Lobby");
      const lobbyDoc = lobbyCol.doc("gYdtPMVaorwoc2jH3Iog");
      return lobbyDoc.collection("room_members").get()
          .then((snapshot) => {
            let size = 0;
            console.log("**** getting snapshot data! ****");
            snapshot.forEach((doc) => {
              console.log(doc.id, " => ", doc.data());
              size = size + 1;
            });
            console.log("there are " + size + " players in lobby");
            // if enough players in lobby: create a new game & delete
            // the players from the lobby
            if (size >= InGamePlayersAmount) {
              console.log("size >= " + InGamePlayersAmount +
              ", creating new game!");
              // create a new game
              // delete players from lobby
            }
          });
    });

// create a new game - currently hard codded - work needed!
exports.createGame = functions.https.onRequest((request, response) => {
  console.log("creating game!");
  const newGameRef = db.collection("Games").doc();
  console.log("new game ref" + newGameRef);
  const newGameMembersRef = newGameRef.collection("room_members");
  console.log("newGameMembersRef" + newGameMembersRef);
  return newGameMembersRef.doc("FKBQ6nK7pRcltvGYiKqB").set({
    userName: "matityahu",
  });
});
