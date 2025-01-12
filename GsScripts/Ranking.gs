function doPost(e) {
  try {
    var params = JSON.parse(e.postData.contents); // Unity 側で送信した JSON データを取得
    var ss = SpreadsheetApp.openById("スプレッドシートのID");
    var sheet = ss.getSheetByName('シート1');
    var ngSheet = ss.getSheetByName('NGワード'); // NGワードシートを取得

    // NGワードシートからNGワードリストを取得
    var ngWords = ngSheet.getRange(1, 1, ngSheet.getLastRow(), 1).getValues(); // 1列目のNGワードをリストで取得
    var ngWordsSet = new Set(ngWords.flat()); // Setに変換して検索を高速化

    // 名前がNGワードリストに含まれているかをチェック
    var name = params.Name;
    if (ngWordsSet.has(name)) {
      name = "*****"; // 名前をNGワードに変更
    }

    // スプレッドシートの次の空行を取得
    var row = sheet.getLastRow() + 1;

    // 名前を変更した場合でも書き込み処理
    sheet.getRange(row, 1).setValue(name); // 名前を列1に書き込む
    sheet.getRange(row, 2).setValue(params.Score); // スコアを列2に書き込む

    var range = sheet.getRange(2, 1, sheet.getLastRow() - 1, 2); // ヘッダー行を除いたデータ範囲を取得
    range.sort({ column: 2, ascending: false });

    var output = ContentService.createTextOutput();
    output.setMimeType(ContentService.MimeType.JSON);
    output.setContent(JSON.stringify({ message: "Success", status: 200 }));
    return output;

  } catch (error) {
    var output = ContentService.createTextOutput();
    output.setMimeType(ContentService.MimeType.JSON);
    output.setContent(JSON.stringify({ message: "Error", details: error.message, status: 500 }));
    return output;
  }
}

function doGet(e) {
  try {
    var ss = SpreadsheetApp.openById("スプレッドシートのID");
    var sheet = ss.getSheetByName('シート1');
    var data = sheet.getDataRange().getValues();

    var playerDataArray = [];
    for (var i = 1; i < data.length; i++) { // ヘッダー行をスキップ
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
