# 🎓 نظام إدارة التدريب التعاوني (Internship Management System)

نظام متكامل لإدارة التدريب التعاوني للطلاب، مع دعم لأربع أدوار مختلفة: **الطلاب، المشرفين الميدانيين، المشرفين الأكاديميين، والمسؤولين**.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-blue.svg)](https://www.mysql.com/)
[![Next.js](https://img.shields.io/badge/Next.js-14-black.svg)](https://nextjs.org/)
[![JWT](https://img.shields.io/badge/JWT-Auth-green.svg)](https://jwt.io/)

---

## 📋 المحتويات

- [المقدمة](#-المقدمة)
- [الميزات](#-الميزات)
- [الهياكل](#-الهياكل)
- [الواجهة الخلفية](#-الواجهة-الخلفية-backend)
- [الواجهة الأمامية](#-الواجهة-الأمامية-frontend)
- [متطلبات النظام](#-متطلبات-النظام)
- [الإعداد](#-الإعداد)
- [البيئة المحلية](#-البيئة-المحلية)
- [الواجهات البرمجية (API)](#-الواجهات-البرمجية-api)
- [المساهمة](#-المساهمة)
- [الرخصة](#-الرخصة)

---

## 🌟 المقدمة

نظام إدارة التدريب التعاوني هو تطبيق ويب يهدف إلى تسهيل إدارة وتوثيق عمليات التدريب التعاوني للطلاب. يوفر النظام واجهة متكاملة تسمح بتسجيل الطلبات، إدارة المهام، تقديم التقارير، وتقديم التقييمات من قبل المشرفين الميدانيين والأكاديميين.

---

## 🚀 الميزات

- ✅ **نظام مصادقة متكامل** (JWT)
- ✅ **إدارة المستخدمين حسب الأدوار**
  - طالب
  - مشرف ميداني
  - مشرف أكاديمي
  - مسؤول
- ✅ **إدارة طلبات التدريب**
- ✅ **نظام المهام**
- ✅ **نظام التقارير الأسبوعية**
- ✅ **نظام التقييمات**
- ✅ **نظام الرسائل الداخلية**
- ✅ **نظام رفع وتنزيل الملفات**
- ✅ **نظام سجلات الأنشطة**
- ✅ **واجهة مستخدم متجاوبة (React/Next.js)**

---

## 🏗️ الهياكل

### قاعدة البيانات

- **MySQL** (8.0+)
- **Entity Framework Core** (8.0)
- **Pomelo.EntityFrameworkCore.MySql**

### الواجهة الخلفية (Backend)

- **ASP.NET Core 8 Web API**
- **C#**
- **Clean Architecture (أساسي)**

### الواجهة الأمامية (Frontend)

- **Next.js** (14)
- **TypeScript**
- **Tailwind CSS**
- **React Hook Form**
- **Axios**

---

## 🛠️ الواجهة الخلفية (Backend)

### الكيانات (Entities)

- `User` - المستخدم العام (طالب، مشرف، إلخ)
- `Student` - الطالب
- `Supervisor` - المشرف (مجال أو أكاديمي)
- `Internship` - التدريب
- `Task` - المهام
- `WeeklyReport` - التقارير الأسبوعية
- `Evaluation` - التقييمات
- `Message` - الرسائل
- `ActivityLog` - سجلات الأنشطة

### الوحدات المُنفّذة

- [x] المصادقة (Auth)
- [x] إدارة الطلاب (Student Management)
- [x] إدارة المشرفين الميدانيين (Site Supervisor Management)
- [x] إدارة المشرفين الأكاديميين (Academic Supervisor Management)
- [x] إدارة التدريبات (Internship Management)
- [x] إدارة المهام (Task Management)
- [x] إدارة التقارير الأسبوعية (Weekly Report Management)
- [x] إدارة التقييمات (Evaluation Management)
- [x] إدارة الرسائل (Internal Messaging)
- [x] إدارة المستخدمين من قبل المشرف (Admin User Management)
- [x] رفع وتنزيل الملفات (File Uploads)
- [x] سجلات الأنشطة (Activity Logs)

---

## 🌐 الواجهة الأمامية (Frontend)

- **Next.js 14 App Router**
- **TypeScript**
- **Tailwind CSS** (UI Framework)
- **Axios** (API Client)
- **React Hook Form** (Form Management)
- **JWT** (Authentication)

---

## 🧰 متطلبات النظام

### الواجهة الخلفية

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0+](https://www.mysql.com/downloads/)
- [XAMPP](https://www.apachefriends.org/download.html) (اختياري)

### الواجهة الأمامية

- [Node.js 18+](https://nodejs.org/)
- [npm](https://www.npmjs.com/) أو [yarn](https://yarnpkg.com/)

---

```markdown
## 🚀 الإعداد

### 1. استنساخ المشروع

```bash
git clone https://github.com/your-username/internship-management-system.git
cd internship-management-system
```

### 2. إعداد الواجهة الخلفية

#### أ. إنشاء قاعدة البيانات

1. افتح XAMPP Control Panel.
2. شغّل Apache و MySQL.
3. افتح phpMyAdmin.
4. أنشئ قاعدة بيانات باسم: `internship_management_db`.

#### ب. تهيئة المشروع

```bash
cd InternshipManagement.API
dotnet restore
```

#### ج. تهيئة المتغيرات البيئية

انسخ `appsettings.json` إلى `appsettings.Development.json` وقم بتعديل Connection String:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=internship_management_db;User=root;Password=;CharSet=utf8mb4;"
  }
}
```

> ⚠️ استخدم `Password=your_password` إذا كنت تستخدم كلمة مرور في MySQL.

#### د. تطبيق الهجرات (Migrations)

```bash
dotnet ef database update
```

#### هـ. تشغيل الخادم

```bash
dotnet run
```

الخادم يعمل على: `http://localhost:5221`
🌐 English Version
Internship Management System API
A full-featured backend for managing cooperative training programs in universities. Built with ASP.NET Core 8, MySQL, and JWT authentication. Supports students, supervisors, and admins with secure role-based access.

Features
Student internship requests

Weekly reports and evaluations

Task management

Internal messaging

File uploads and downloads

Admin user management

Activity logging middleware

Technologies
ASP.NET Core 8

Entity Framework Core

MySQL

JWT

AutoMapper

RESTful APIs
