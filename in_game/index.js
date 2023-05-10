const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

// // Create and deploy your first functions
// // https://firebase.google.com/docs/functions/get-started
//
exports.updateObject = regionalFunctions.https.onCall(async(data) => {
  console.log("***********updateObject*********");
  const info = data;
  const userID = info.userID;
  const roomId = info.roomID;
  const objectName = info.objectName;
  const isOnInput = info.isOn;
  let oldState;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();
  console.log("number of docs: " + docs.size);
  docs.forEach(async (doc) =>{
    oldState = doc.data().isOn;
    await doc.ref.update({
        isOn : !oldState
    });
  });
  return "response from 'updateObject': isOn: Before = " + oldState + "After = " + !oldState;
});



function successfulRequest(data) {
    return {
      isFaulted: false,
      data: data,
    };
  }
  
  function failedRequest(errorMessage) {
    return {
      isFaulted: true,
      error: errorMessage,
    };
  }