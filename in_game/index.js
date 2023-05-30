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
      const p = doc.ref.update({
        location : null,//location = null
        [key] : userId, //updating owner
      });
      promises.push(p);
    }
  });
  await Promise.all(promises);
  console.log("about to return: " + isPickedUp);
  return isPickedUp;
});

exports.pickUpPanel = regionalFunctions.https.onCall(async(data) => {
  console.log("***********pickUpPanel*********");
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
    if(currentOwner == null){ // panel is free to grab.
      isPickedUp = true;
      const p = doc.ref.update({
        [key] : userId, //updating owner
      });
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

  const level = data.data.level;
  const playerLocationX = data.data.playerLocationX;
  const playerLocationZ = data.data.playerLocationZ;
  const playerDirectionX = data.data.playerDirectionX;
  const playerDirectionZ = data.data.playerDirectionZ;
  const desiredY = data.data.desiredY

  console.log("Player's locationX: " + playerLocationX);
  console.log("Player's locationZ: " + playerLocationZ);
  console.log("Player's directionX: " + playerDirectionX);
  console.log("Player's directionZ: " + playerDirectionZ);
  console.log("current level:" + level);



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
      const levelData = await db.collection("Levels").doc(level).get(); 
      const newLocation = getValidLocation(playerLocationX,playerLocationZ,2*playerDirectionX, 2*playerDirectionZ ,levelData.data(), desiredY);
      console.log("location chosen for item: " + newLocation);
      const p = doc.ref.update({
        [key] : null,
        location : newLocation
      });
      promises.push(p);
    }
  });
  await Promise.all(promises);
  console.log("about to return: " + isDropped);
  return isDropped;
});

exports.dropPanel = regionalFunctions.https.onCall(async(data) => {
  console.log("***********dropPanel*********");
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
    if(currentOwner == userId){ // panel is free to drop.
      console.log("entered 'if'. about to drop panel.");
      isDropped = true;
      const p = doc.ref.update({
        [key] : null,
      });
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
  const key = data.data.key; // isOpen
  const objectName = data.objectName; 
  const codeInput = data.data.code;
  let isCodeValid = false;
  const docs = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', objectName).get();

  promises = [];
  docs.forEach(async (doc) =>{
    let code = doc.data().code;
    console.log("input by user: " + codeInput);
    if (objectName == "board"){
      const s = doc.ref.update({
        currentRooksLocations : code,
      });
      promises.push(s);
    }
    console.log("code in database: " + code);
    if(code.toString() == codeInput.toString()){ // code is correct
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

function getValidLocation(playerLocationX, playerLocationZ, playerDirectionX, playerDirectionZ, levelData, desiredY) {
  let borderX, borderZ;
  if(playerLocationZ <= 0){
    borderX = levelData.roomBordersX;
    borderZ = levelData.roomBordersZ;
  } else{
    borderX = levelData.roomBordersX2;
    borderZ = levelData.roomBordersZ2;
  }
  let desiredLocationX = playerLocationX + playerDirectionX;
  let desiredLocationZ = playerLocationZ + playerDirectionZ;

  for(let i = 0 ; i <= 4 ; i++){
    switch (i){
      case 1: {
        desiredLocationX = desiredLocationX - (2*playerDirectionX);
        break;
      }
      case 2: {
        desiredLocationZ = desiredLocationZ - (2*playerDirectionZ);
        break;
      }
      case 3: {
        desiredLocationX = desiredLocationX + (2*desiredLocationX);
        break;
      }
      case 4: {
        desiredLocationX = getClosestBorder(playerLocationX, borderX);
        desiredLocationZ = getClosestBorder(playerLocationZ, borderZ);
        console.log("player position: " + playerLocationX + " " + playerLocationZ);
        console.log("closest border: " + desiredLocationX + " " + desiredLocationZ);
        return getLocation(desiredLocationX,desiredY, desiredLocationZ);
      }
    }
    console.log("checking: " + getLocation(desiredLocationX,desiredY,desiredLocationZ));
    if(checkIfLocationIsValid(desiredLocationX,desiredLocationZ,borderX,borderZ)){
      const location =  getLocation(desiredLocationX,desiredY,desiredLocationZ);
      console.log("location is valid! returning: " + location);
      return location;
    } else{
      console.log("position invalid. x: " + borderX[0] + " " + desiredLocationX + " " + borderX[1]);
      console.log("position invalid. z: " + borderZ[0] + " " + desiredLocationZ + " " + borderZ[1]);
    }
  }



  // console.log("checking: " + getLocation(desiredLocationX,desiredY,desiredLocationZ));
  // if(checkIfLocationIsValid(desiredLocationX,desiredLocationZ,borderX,borderZ)){
  //   return getLocation(desiredLocationX,desiredY,desiredLocationZ);
  // }
  // desiredLocationX = desiredLocationX - (2*playerDirectionX);
  // console.log("checking: " + getLocation(desiredLocationX,desiredY,desiredLocationZ));
  // if(checkIfLocationIsValid(desiredLocationX,desiredLocationZ,borderX,borderZ)){
  //   return getLocation(desiredLocationX,desiredY,desiredLocationZ);
  // }
  // desiredLocationZ = desiredLocationZ - (2*playerDirectionZ);
  // console.log("checking: " + getLocation(desiredLocationX,desiredY,desiredLocationZ));
  // if(checkIfLocationIsValid(desiredLocationX,desiredLocationZ,borderX,borderZ)){
  //   return getLocation(desiredLocationX,desiredY,desiredLocationZ);
  // }
  // desiredLocationX = desiredLocationX + (2*desiredLocationX);
  // console.log("checking: " + getLocation(desiredLocationX,desiredY,desiredLocationZ));
  // if(checkIfLocationIsValid(desiredLocationX,desiredLocationZ,borderX,borderZ)){
  //   return getLocation(desiredLocationX,desiredY,desiredLocationZ);
  // }
  // console.log("problem with locations, no location was found valid!");
  // if(Math.abs(borderX[0] - playerLocationX) <= Math.abs(borderX[1] - playerLocationX)){
  //   desiredLocationX = borderX[0];
  // } else{
  //   desiredLocationX = borderX[1];
  // }

  // if(Math.abs(borderZ[0] - playerLocationZ) <= Math.abs(borderZ[1] - playerLocationZ)){
  //   desiredLocationZ = borderZ[0];
  // } else{
  //   desiredLocationZ = borderZ[0];
  // }
  // // desiredLocationX = Math.min(Math.abs(borderX[0] - playerLocationX) ,Math.abs(borderX[1] - playerLocationX));
  // // desiredLocationZ = Math.min(Math.abs(borderZ[0] - playerLocationZ) ,Math.abs(borderZ[1] - playerLocationZ));
  // const location = getLocation(desiredLocationX,desiredY,desiredLocationZ);
  // console.log("returning: " + location);
  // return location;
}

function checkIfLocationIsValid(x,z,borderX,borderZ){
  return (borderX[0]<=x && x <= borderX[1]) && (borderZ[0]<=z && z<=borderZ[1]);
}

function getLocation(x,y,z){
  return "(" + x.toFixed(2) + ", " + y + ", " + z.toFixed(2) + ")";
}

function getClosestBorder(playerLocation, border){
  if(Math.abs(border[0] - playerLocation) <= Math.abs(border[1] - playerLocation)){
    return border[0];
  } else{
    return border[1];
  }
}