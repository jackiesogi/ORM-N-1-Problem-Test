# ORM N+1 Problem Test

![縮圖](./wwwroot/img/demo.png)

## 簡介
透過實作簡易的購物系統，研究 ORM 語法造成的 N+1 問題。
實作兩個 Handler：`GetCartItemNPlusOne()` 及 `GetCartItemOptimized()`，並利用執行時間（毫秒）來比較查詢效率。

### 原始碼位置
- [GetCartItemNPlusOne()](https://github.com/jackiesogi/ORM-N-1-Problem-Test/blob/master/Controllers/DemoController.cs#L109-L135)
- [GetCartItemOptimized()](https://github.com/jackiesogi/ORM-N-1-Problem-Test/blob/master/Controllers/DemoController.cs#L137-L162)

## 在 Ubuntu Linux 手動建置

### 下載本專案
```bash
git clone https://github.com/jackiesogi/ORM-N-1-Problem-Test.git
cd ORM-N-1-Problem-Test
```

### 安裝相依套件
```bash
sudo apt update
sudo apt install sqlite3
sudo apt-get install -y dotnet-sdk-8.0
sudo apt-get install -y aspnetcore-runtime-8.0
sudo apt-get install -y dotnet-runtime-8.0
```

### 獲取 .NET 相依套件
```bash
# entity framework
dotnet add package Microsoft.EntityFrameworkCore.Design
# sqlite support
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Data.Sqlite
# json support
dotnet add package System.Text.Json
dotnet add package System.Text
```

### 資料庫內容
可以使用 Entity Framework Core 進行資料庫遷移，以還原資料模型
```bash
dotnet ef database update
```
或是直接使用預設的資料庫檔案，已經位於專案的 `db` 路徑中，檔案也完整保留在檔案中，無須額外動作。

### 編譯專案
```bash
dotnet build
dotnet run
```
完成編譯後，開啟任何瀏覽器，輸入 http://localhost:5015/Demo 查看本專案的詳細資訊；或是您也可以造訪 https://jackiesogi.com/Demo 來查看線上的 Demo 版本，省去編譯的時間。
