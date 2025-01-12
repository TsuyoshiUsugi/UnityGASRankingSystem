const CONFIG = {
  SPREADSHEET_ID: "your_spreadsheet_id",
  SHEETS: {
    MAIN: "Main",
    NG_WORDS: "BadWords"
  }
};

function doGet(e) {
  try {
    var ss = SpreadsheetApp.openById(CONFIG.SPREADSHEET_ID);
    var sheet = ss.getSheetByName(CONFIG.SHEETS.MAIN);
    var data = sheet.getDataRange().getValues();

    var playerDataArray = [];
    for (var i = 1; i < data.length; i++) {
      var playerData = {
        Name: data[i][0],
        Score: data[i][1]
      };
      playerDataArray.push(playerData);
    }

    var response = {
      PlayerDataArray: playerDataArray
    };

    return ContentService.createTextOutput(JSON.stringify(response)).setMimeType(ContentService.MimeType.JSON);

  } catch (error) {
    var errorResponse = { message: "Error", details: error.message };
    return ContentService.createTextOutput(JSON.stringify(errorResponse)).setMimeType(ContentService.MimeType.JSON);
  }
}

function doPost(e) {
  try {
    var params = JSON.parse(e.postData.contents);
    var action = params.Action;
    var data = params.Data;

    if (action === "addRanking") {
      var rankingData = JSON.parse(data); 
      return addRanking(rankingData);
    } else if (action === "deleteRanking") {
      return deleteRanking();
    } else {
      throw new Error("Invalid action");
    }
  } catch (error) {
    return ContentService.createTextOutput(
      JSON.stringify({ message: "Error", details: error.message, status: 500 })
    ).setMimeType(ContentService.MimeType.JSON);
  }
}

function addRanking(data) {
  var ss = SpreadsheetApp.openById(CONFIG.SPREADSHEET_ID);
  var sheet = ss.getSheetByName(CONFIG.SHEETS.MAIN);
  var ngSheet = ss.getSheetByName(CONFIG.SHEETS.NG_WORDS); 

  var ngWords = ngSheet.getRange(1, 1, ngSheet.getLastRow(), 1).getValues();
  var ngWordsSet = new Set(ngWords.flat()); 
  if (ngWordsSet.has(data.Name)) {
    data.Name = "*****"; 
  }

  var row = sheet.getLastRow() + 1;
  sheet.getRange(row, 1).setValue(data.Name);
  sheet.getRange(row, 2).setValue(data.Score);

  var range = sheet.getRange(2, 1, sheet.getLastRow() - 1, 2);
  range.sort({ column: 2, ascending: false });

  return ContentService.createTextOutput(
    JSON.stringify({ message: "Score added and sorted", status: 200 })
  ).setMimeType(ContentService.MimeType.JSON);
}

function deleteRanking() {
  var ss = SpreadsheetApp.openById(CONFIG.SPREADSHEET_ID);
  var sheet = ss.getSheetByName(CONFIG.SHEETS.MAIN);
  sheet.getRange(2, 1, sheet.getLastRow() - 1, 2).clearContent();

  return ContentService.createTextOutput(
    JSON.stringify({ message: "Ranking data cleared", status: 200 })
  ).setMimeType(ContentService.MimeType.JSON);
}