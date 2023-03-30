const functions = require("firebase-functions");

const regionalFunctions = functions.region("europe-west1");
exports.helloWorld = regionalFunctions.https.onRequest((request, response) => {
  functions.logger.info("Hello logs!", {structuredData: true});
  response.send("Hello from Firebase!");
});


