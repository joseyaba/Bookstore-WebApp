# 📚 Bookstore Web Application

A full-stack web application that allows users to register/login and manage their own books. Built with **.NET Core Web API** and **React**.

---

## 📁 Project Structure

- `BookStore.API` – Backend API built with ASP.NET Core
- `BookStore.Tests` – Unit tests for backend (xUnit & Moq)
- `bookstore-client` – Frontend built with React

---

## 🚀 How to Run the Project

### 🖥 Backend (BookStore.API)
1. Open a terminal and navigate to the `BookStore.API` folder.
2. Run the following command:

```
dotnet run
```

> This will start the backend on `https://localhost:7045/api` (check terminal output to confirm).

---

### 🌐 Frontend (bookstore-client)
1. Open a terminal and navigate to the `bookstore-client` folder.
2. Run the following command:

```
npm install
npm start
```

> This will launch the frontend at `http://localhost:3000`

Make sure the backend is running before logging in.

---

## ✅ Running Unit Tests

1. Open a terminal and navigate to the `BookStore.Tests` folder.
2. Run this command:

```
dotnet test
```

> All your backend unit tests will be executed.

---

## 🔐 Authentication

- Users register and login via the `/auth` API endpoints.
- On successful login, a **JWT token** is returned.
- The token is saved in browser storage and attached to every API request.
- Only logged-in users can access `/book` endpoints.

---

## 🛠 Technologies Used

- **ASP.NET Core** (.NET 6)
- **Entity Framework Core** (In-memory for testing)
- **React** (Create React App)
- **Bootstrap** (UI Styling)
- **JWT (JSON Web Tokens)** – Token-based authentication
- **xUnit & Moq** – For backend unit testing

---

## 👩🏽‍💻 Author

Developed with ❤️ by **Josey**
