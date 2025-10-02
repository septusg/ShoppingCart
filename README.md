# ShoppingCart
For demonstration purposes only.  
用於展示專案  
　　

## 專案簡介  
  
一個示範用購物車功能，前後端分離：  

・前端：Vue 3 + Vite + Pinia（狀態管理）｜使用 Tailwind CSS 來做樣式  
・後端：ASP.NET Core Web API（使用 DTO、JWT 驗證）  
・資料庫：MSSQL（EF Core Migrations）  
  
  

## 主要功能  
・註冊 / 登入（JWT）  
・商品（Product）列表與 CRUD（API）  
・購物車（Cart）：新增、修改數量、刪除  
・後端使用 DTO 避免序列化循環／過度暴露資料  
・前端使用 Pinia 作為全域狀態（管理 cart / auth）  
・Swagger 提供 API 文件與測試介面  
  
  

## 專案結構  
```
/ShoppingCart  
  Controllers/  
  Models/  
  Models/Dto/  
  Program.cs  
  appsettings.json  
  
/frontend  
  src/  
    pages/  
    stores/  
    services/api.js  
  ```
  

## 開發環境（本機）  
・.NET SDK｜8.0.414 (x64)  
・Node.js｜v24.9.0  
・npm｜11.6.1  
・MSSQL Server  
  
・工具：VS2022、VS Code
  
  

## 預設 Demo 帳號  
  
預設測試使用者  
・Email: abc@gmail.com  
・Password: abc123  
  
預設商品  
・貓貓耳機  
・狗狗滑鼠
  
  
## 快速測試：Docker（已整合，推薦用此方式啟動）
已把前端 build 成靜態檔並用 nginx 服務，後端用 ASP.NET Core container，DB 用 MSSQL container  
> 執行前需先啟動 Docker Desktop、確定 WSL 是否更新  
### 啟動（cmd，在專案根目錄，含 docker-compose.yml 的資料夾）
```powershell
# 第一次或有改 image 時（包括修改前端 tailwind 或 VITE_API_URL）
docker-compose up --build

# 之後一般啟動（不用 rebuild）
docker-compose up
```

### 停掉與清除
```powershell
# 只停掉容器（保留 volume）
docker-compose down

# 連同 volumes 一起刪（會刪掉 DB 資料）
docker-compose down -v

# 若要連 images 一起刪（最乾淨）
docker-compose down --rmi all -v
```

### 重新 build 單一服務
```powershell
# 只重 build 前端 image
docker-compose build frontend

# 只重 build 後端 image
docker-compose build app
```
  

## 建議 Demo 流程
> 確認 Docker build 成功後，連上前端網頁：http://localhost:5173/
  
1. 用 Swagger 或前端 /register 註冊 demo 帳號  
  
2. 登入 /login（前端或 Swagger），確認取得 token  
  
3. 在商品頁點「加入購物車」，確認 header 購物車數字即時變動  
  
4. 前往 /cart，測試更新數量與刪除，觀察後端 API 呼叫與回應
  
  

### 重要設定說明
- 前端 build-time env：VITE_API_URL 是 build 時注入的變數，若改了要重新 build 前端 image 才會生效  
> （例如 `docker-compose up --build` 或 `docker-compose build frontend`）  
- 前端 container 使用 nginx（在 container 內監聽 80），compose 已把主機 port `5173` 對應到 container 的 `80`（`5173:80`）  
- 若把 VITE_API_URL 設成 `http://localhost:7055/api`，瀏覽器會直接向主機的 7055 port 請求；若改成 `http://app:7055/api`，只有在 container network 內可解析（通常只在 container 內有用）  
  

  
## 本機啟動（開發模式）  
### 設定後端環境變數 / appsettings  
 
在 appsettings.json 或系統環境變數設定下列值（範例）：  
```json
{  
  "ConnectionStrings": {  
    "DefaultConnection": "Server=localhost;Database=ShoppingCart;User Id=sa;Password=YourStrong!Passw0rd;"  
  },  
  "Jwt": {  
    "Key": "請換成至少 16 bytes（建議 32 bytes）的安全字串",  
    "Issuer": "ShoppingCartApi",  
    "Audience": "ShoppingCartClient",  
    "ExpiresInMinutes": 60  
  }  
}  
```
> 正式環境不放這  
  

### 資料庫遷移（Migrations）  
在後端專案目錄執行：  
```
# 若已存在 migration 可跳過  
dotnet ef migrations add Init  
  
dotnet ef database update  
```
  
  
### 啟動後端  
```powershell
dotnet run
```  
或用 Visual Studio 2022 執行(F5)  
  
[Swagger](https://localhost:7055/swagger/index.html)  
  

### 啟動前端  
```powershell
cd frontend  
npm install  
npm run dev  
```
前端開發伺服器通常為 http://localhost:5173/  
  
  

## 常見問題與排除建議  
・EF 找不到 AddDbContext / 命名空間錯誤：  
　請確認以下 Nuget 套件已安裝  
　　。Microsoft.EntityFrameworkCore  
　　。Microsoft.EntityFrameworkCore.SqlServer  
　　。Microsoft.EntityFrameworkCore.Tools  
並 dotnet restore、重新 build  
  
・JWT 產生 500（Key 太短）：  
請確保 Jwt:Key 至少 16 bytes（建議 32 bytes）  
可使用 PowerShell 產生安全 key  
  
・跨域 / CORS 問題：  
若 API 回應被瀏覽器擋掉，檢查後端是否開放 `http://localhost:5173` 的 CORS  
  
・前端無法連到後端：  
檢查 src/services/api.js 的 baseURL 是否指向正確後端 URL（包含 https 與 port）  
  
・Network Error / 前端抓不到 API：
通常是前端用的 baseURL 指向了 container 內的 service name（例如 `http://app:7055`），但瀏覽器無法解析
解法：將前端的 VITE_API_URL 設成能被瀏覽器存取的 URL（例如 `http://localhost:7055/api`）並重建前端 image
  
  
