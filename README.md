# 📋 Contract Monthly Claim System (CMCS)

A web-based application for managing monthly claims submission and approval processes for Independent Contractor lecturers.

---

## 🚀 Features

- **User Authentication** — JWT-based login system  
- **Role-based Access Control** — Admin, Academic Manager, Programme Coordinator, Lecturer  
- **Claims Management** — Submit, view, and track claims  
- **Document Upload** — Support for PDF, DOCX, XLSX files (max 2MB)  
- **Automated Calculations** — Automatic total amount calculation  
- **Approval Workflow** — Multi-level approval process  

---

## 🛠️ Tech Stack

### 🧩 Backend
- ASP.NET Core **9.0** Web API  
- Entity Framework Core (Code-First + SQLite)  
- JWT Authentication  
- xUnit Tests  

### 💻 Frontend
- Vue.js **3** with TypeScript  
- Nuxt UI + Tailwind CSS  
- Pinia (state management)  
- ofetch (HTTP client)

---

## 📁 Project Structure

```
PROG6212-CMCS/
├── PROG6212-CMCS.Server/          # ASP.NET Core Backend
│   ├── Controllers/               # API Controllers
│   ├── Models/                    # Data Models
│   ├── Data/                      # DbContext & EF Migrations
│   ├── Program.cs                 # Application Entry Point
│   └── appsettings.json           # Configuration & JWT
│
├── prog6212-cmcs.client/          # Vue 3 + TypeScript Frontend
│   ├── components/                # UI Components
│   ├── views/                     # Routes
│   ├── stores/                    # Pinia Stores
│   ├── composables/               # API & Helper Functions
│   └── package.json               # Frontend Dependencies
│
├── PROG6212_CMCS.Tests/           # Unit Test Project (xUnit)
└── README.md
```

---

## ⚙️ Quick Start

### 📦 Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQLite (included with .NET)

---

### 🧰 Installation & Setup

#### 1️⃣ Clone the repository
```bash
git clone <repository-url>
cd PROG6212-CMCS
```

#### 2️⃣ Setup Backend
```bash
cd PROG6212-CMCS.Server
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate // if doesn't have
dotnet ef database update
```

#### 3️⃣ Setup Frontend
```bash
cd ../prog6212-cmcs.client
npm install
```

#### 4️⃣ Run the Project
```bash
cd ../PROG6212-CMCS.Server
dotnet run
```


## 🔐 Default Login Credentials

| Role | Email | Password |
|------|--------|----------|
| Admin | admin@gmail.com | 12345678 |
| Academic Manager | manager@gmail.com | 12345678 |
| Programme Coordinator | coordinator@gmail.com | 12345678 |
| Lecturer | lecturer@gmail.com | 12345678 |

---

## 🧩 Troubleshooting

### ❗ Missing Assembly or Library (Backend)
```bash
cd PROG6212-CMCS.Server
dotnet clean
dotnet restore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
```

### ❗ Frontend Dependency Issues
```bash
cd prog6212-cmcs.client
rm -rf node_modules
npm cache clean --force
npm install
```

### ❗ Database Issues
```bash
cd PROG6212-CMCS.Server
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🧪 Testing

Run all unit tests:
```bash
cd PROG6212_CMCS
dotnet test
```

---

## 🧠 Notes

- Uploaded files are stored under:  
  `wwwroot/uploads/claims/{claimId}/`
- Claims can be approved by either **Programme Coordinator** or **Academic Manager**.
- Lecturer’s total claim value is auto-calculated using:  
  `TotalAmount = HoursWorked * Lecturer.HourlyRate`

---

## 👨‍💻 Author
**Miguel Domingos**   
PROG6212 — Contract Monthly Claim System (CMCS) - POE 2
