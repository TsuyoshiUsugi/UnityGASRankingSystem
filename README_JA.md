# UnityGASランキングシステム  
これは、GoogleAppsScriptを使用したUnity向けランキングシステムです。

## セットアップ

### 必要要件

* Unity 2022.3以降  
2022以前でも動作する可能性があります。

### インストール

1. Window > Package ManagerからPackage Managerを開きます。
2. 「+」ボタンをクリックし、「Add package from git URL」を選択します。
3. 次のURLを入力します：

```
https://github.com/TsuyoshiUsugi/UnityGASRankingSystem.git
```

## スプレッドシートの準備

1. **Googleスプレッドシートをセットアップ**
   - Googleドライブ内の`SpreadSheet`フォルダにスプレッドシートを配置します。

2. **Apps Scriptを開く**
   - スプレッドシートを開き、メニューから「拡張機能」→「Apps Script」を選択します。
   ![21d67be3fcf5586a43159def9ef95c4b](https://github.com/user-attachments/assets/79653538-0b07-42ef-8eb3-2edb44cf2416)

   - Apps Scriptエディタが開き、コードを記述できます。

3. **Apps Scriptにコードを貼り付ける**
   - `GsScripts`フォルダ内の`Ranking.gs`ファイルを開きます。
   - `Ranking.gs`の内容をApps Scriptエディタにコピー＆ペーストします。

   コードを貼り付けた後、以下の設定を行います：

     ```javascript
     const CONFIG = {
       SPREADSHEET_ID: "Your Sheet ID",
       SHEETS: {
         MAIN: "Sheet1",
         NG_WORDS: "NG Words"
       }
     };
     ```

   - **SPREADSHEET_IDを設定**
     - スプレッドシートの「共有」からリンクをコピーします。
     - リンクの中の`https://docs.google.com/spreadsheets/d/`と`/edit?usp=sharing`の間にあるIDを抜き出します。

4. **Apps Scriptをデプロイする**
   - Apps Scriptエディタの「デプロイ」ボタンをクリックします。
   - 「新しいデプロイ」を選択し、デプロイを完了します。

これでスプレッドシートのセットアップは完了です。

---

## 使用方法

### Unityセットアップ

1. **GASManager.prefabをシーンに追加**
   - `Runtime`フォルダ内の`GASManager.prefab`をランキング機能を使用したいシーンに配置します。

2. **GASRankingManagerインスタンスにアクセス**
   - `GASRankingManager.Instance`を使用してさまざまな機能を呼び出します。

### 機能

#### スコア送信

プレイヤーの名前とスコアを以下のコードで送信できます：

```csharp
GASRankingManager.Instance.SendScore(_nameInputField.text, score);
```

- **引数**
  - `name`: プレイヤー名（`string`型）
  - `score`: スコア（`float`型）

#### スコアリストの取得

以下のコードでスコアリストを取得して表示できます：

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

- **引数**
  - スコア表示順（昇順または降順）：`GASRankingManager.GetScoreOrder`
  - 成功時コールバック（`Action<List<PlayerData>>`）
  - 失敗時コールバック（`Action`）

#### ランキングの削除（エディタ専用）

- **手順**
  - `GASManager.prefab`のInspectorに表示される削除ボタンをクリックします。
![716729c061d3514532850f9cc08fd3cd](https://github.com/user-attachments/assets/8d12c0bd-cb9d-4cd3-8c5f-d26ef9e6fd21)

---
