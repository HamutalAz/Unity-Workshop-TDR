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
  const roomId = info.roomID;
  const key = info.data.key;
  const objectName = info.objectName; 
  let oldState;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();
  console.log("number of docs: " + docs.size);

  docs.forEach(async (doc) =>{
    console.log(String(key));
    oldState = doc.get(key);

    await doc.ref.update({
      [key] : !oldState
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