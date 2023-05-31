const functions = require("firebase-functions");
const regionalFunctions = functions.region("europe-west1");
const firestore = regionalFunctions.firestore;
const admin = require("firebase-admin");
admin.initializeApp();
const db = admin.firestore();

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
  const isSpecificLocation = data.data.dropInLoc;
  let specificLocation = null;
  if(isSpecificLocation){
    specificLocation = data.data.specificLocation;
  }
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
      if(!isSpecificLocation){
        specificLocation = getValidLocation(playerLocationX,playerLocationZ,2*playerDirectionX, 2*playerDirectionZ ,levelData.data(), desiredY);
      }
      //const newLocation = getValidLocation(playerLocationX,playerLocationZ,2*playerDirectionX, 2*playerDirectionZ ,levelData.data(), desiredY);
      console.log("location chosen for item: " + specificLocation);
      const p = doc.ref.update({
        [key] : null,
        location : specificLocation
      });
      if(objectName == "plate"){
        const response = await checkIfLocationIsWithinRedLight(specificLocation, roomId);
        console.log(response);
        // promises.push(response);
        if(response){
          console.log("location is within red light");
          const a = doc.ref.update({
            isReadable : true
          });
          promises.push(a);
        } else{
          console.log("location isn't within red light");
           const b = doc.ref.update({
            isReadable : false
          });
          promises.push(b);
        }
      }
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

      if(objectName == "board"){ // open all doors!
        const doors = await db.collection("Rooms").doc(roomId).collection("room_objects")
        .where('name', 'in', ['cellDoor', 'fanDoor']).get();
        console.log("number of doors found in query: " + doors.size);
        doors.forEach(async(door) => {
          console.log("currently opening: " + door.data().name);
          const x = door.ref.update({
            isOpen : true,
          });
          promises.push(x);
        });
      }
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

async function checkIfLocationIsWithinRedLight(specificLocation, roomId){
  let returnValue = false;
  
  const redLightDoc = await db.collection("Rooms").doc(roomId).collection("room_objects")
  .where('name','==', "redLight").get();
  console.log("At: checkIfLocationIsWithinRedLight");
  redLightDoc.forEach((val) => {
    const data = val.data();
    const isOn = data.isOn;
    if(isOn){
      console.log("light is on!");
      returnValue = checkIfLocationIsInCircle(specificLocation, data.center, data.radius);
      console.log("location is within red light!");
    }
    else{
      console.log("location isn't within red light!");
    }
  });
  return Promise.resolve(returnValue);
}


function checkIfLocationIsInCircle(location, center, radius){
  console.log('At: checkIfLocationIsInCircle!');
  const x = stringToPoint(location, 0);
  const z = stringToPoint(location, 2);

  var dist_points = (x - center[0]) * (x - center[0]) + (z - center[1]) * (z - center[1]);
  radius *= radius;
  if (dist_points < radius) {
    return true;
  }
  return false;
}


function stringToPoint(location, index){
  console.log('At: stringToPoint! index: ' + index);
  console.log("before split: " + location);
  const newString = splitMulti(location, [' ', '(', ')']);
  console.log("after split: " + newString);
  const returnValue = parseDouble(newString[index]);
  console.log("about to return: " + returnValue);
  return returnValue;;
}


function splitMulti(str, tokens){
  var tempChar = tokens[0]; // We can use the first token as a temporary join character
  for(var i = 1; i < tokens.length; i++){
      str = str.split(tokens[i]).join(tempChar);
  }
  str = str.split(tempChar);
  return str;
}