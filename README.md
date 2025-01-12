# UnityGASRankingSystem  
This is a ranking system using GoogleAppsScript, intended for use with Unity.  
## Setup

### Requirements

* Unity 2022.3 or later  
Maybe it will work before 2022.

### Installation

1. Open Package Manager from Window > Package Manager.
2. Click the "+" button > Add package from git URL.
3. Enter the following URL:

```
https://github.com/TsuyoshiUsugi/UnityGASRankingSystem.git
```

## Preparing the Spreadsheet

1. **Set Up a Google Spreadsheet**
   - Use a spreadsheet in the `SpreadSheet` folder on Google Drive.

2. **Open Apps Script**
   - Open the spreadsheet, and from the menu, select "Extensions" → "Apps Script."
   - This will open the Apps Script editor where you can write your code.

3. **Paste the Code in Apps Script**
   - Open the `Ranking.gs` file from the `GsScripts` folder.
   - Copy and paste the content of `Ranking.gs` into the Apps Script editor.

   After pasting the code, set up the following configuration:

     ```javascript
     const CONFIG = {
       SPREADSHEET_ID: "Your Sheet ID",
       SHEETS: {
         MAIN: "Sheet1",
         NG_WORDS: "NG Words"
       }
     };
     ```

   - **Set SPREADSHEET_ID**
     - Copy the link to your spreadsheet from "Share."
     - Extract the ID from the link, which is the part between `https://docs.google.com/spreadsheets/d/` and `/edit?usp=sharing`.

4. **Deploy Apps Script**
   - Click the "Deploy" button in the Apps Script editor.
   - Select "New Deployment" and complete the deployment process.

This completes the spreadsheet setup.

---

## Usage

### Unity Setup

1. **Add GASManager.prefab to the Scene**
   - Place the `GASManager.prefab` from the `Runtime` folder into the scene where you want to use the ranking feature.

2. **Access the GASRankingManager Instance**
   - Use `GASRankingManager.Instance` to call various functions.

### Features

#### Sending Scores

You can send a player's name and score using the following code:

```csharp
GASRankingManager.Instance.SendScore(_nameInputField.text, score);
```

- **Arguments**
  - `name`: Player name (`string` type)
  - `score`: Score (`float` type)

#### Retrieving the Score List

You can retrieve and display the score list with the following code:

```csharp
GASRankingManager.Instance.GetScoreList(GASRankingManager.GetScoreOrder.Descending, list =>
{
    foreach (Transform child in _scoreListParent.transform)
    {
        Destroy(child.gameObject);
    }

    foreach (var playerData in list)
    {
        var obj = Instantiate(_scoreListPrefab, _scoreListParent.transform);
        obj.GetComponentsInChildren<Text>()[0].text = playerData.Name;
        obj.GetComponentsInChildren<Text>()[1].text =
            playerData.Score.ToString(CultureInfo.CurrentCulture);
    }
}, () => { Debug.LogError("Fail"); });
```

- **Arguments**
  - Score display order (ascending or descending): `GASRankingManager.GetScoreOrder`
  - Success callback (`Action<List<PlayerData>>`)
  - Failure callback (`Action`)

#### Deleting the Ranking (Editor Only)

- **Steps**
  - Click the delete button displayed in the Inspector of `GASManager.prefab`.

---

This concludes the instructions for using the ranking feature.
