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
  const roomId = data.roomID;
  const key = data.data.key;
  const objectName = data.objectName; 
  let oldState;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();
  console.log("number of docs: " + docs.size);

  promises = [];
  docs.forEach(async (doc) =>{
    console.log(oldState);
    oldState = doc.get(key);
    console.log(oldState);

    const p = doc.ref.update({
      [key] : !oldState
    });
    promises.push(p);
  });
  await Promise.all(promises);
  console.log("About to return: " + !oldState);
  return !oldState;
});

exports.pickUpObject = regionalFunctions.https.onCall(async(data) => {
  console.log("***********pickUpObject*********");
  const userId = data.userID;
  const roomId = data.roomID;
  const key = data.data.key;
  const objectName = data.objectName; 
  let isPickedUp = false;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();

  promises = [];
  docs.forEach(async (doc) =>{
    let currentOwner = doc.data().owner;
    console.log("current owner of object: " + currentOwner);
    if(currentOwner == null){ // object is free to grab.
      console.log("entered 'if'. about to pickUp object.");
      isPickedUp = true;
      const p = doc.ref.update({[key] : userId});
      promises.push(p);
    }
  });
  await Promise.all(promises);
  console.log("about to return: " + isPickedUp);
  return isPickedUp;
});


exports.dropObject = regionalFunctions.https.onCall(async(data) => {
  console.log("***********dropObject*********");
  const userId = data.userID;
  const roomId = data.roomID;
  const key = data.data.key;
  const objectName = data.objectName; 
  let isDropped = false;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();

  promises = [];
  docs.forEach(async (doc) =>{
    let currentOwner = doc.data().owner;
    console.log("current owner of object: " + currentOwner);
    if(currentOwner == userId){ // object is free to drop.
      console.log("entered 'if'. about to drop object.");
      isDropped = true;
      const p = doc.ref.update({[key] : null});
      promises.push(p);
    }
  });
  await Promise.all(promises);
  console.log("about to return: " + isDropped);
  return isDropped;
});

exports.checkCode = regionalFunctions.https.onCall(async(data) => {
  console.log("***********checkCode************");
  const roomId = data.roomID;
  const key = data.data.key;
  const objectName = data.objectName; 
  const codeInput = data.data.code;
  let isCodeValid = false;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();

  promises = [];
  docs.forEach(async (doc) =>{
    let code = doc.data().code;
    console.log("input by user: " + codeInput);
    if(code == codeInput){ // code is correct
      console.log("entered 'if'. code is correct.");
      isCodeValid = true;
      const p = doc.ref.update({
        [key] : isCodeValid, // opening box
        owner : null // releasing panel!
      }); 
      
      promises.push(p);
    }
  });
  await Promise.all(promises);
  console.log("about to return: " + isCodeValid);
  return isCodeValid;
});

