-- MySQL initialization script for Assessment project
-- This script creates the assessment_db database with proper character set

-- Create database if not exists
CREATE DATABASE IF NOT EXISTS assessment_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

-- Use the database
USE assessment_db;

-- Create Courses table
CREATE TABLE IF NOT EXISTS Courses (
  Id CHAR(36) PRIMARY KEY COMMENT 'GUID',
  Title VARCHAR(255) NOT NULL COMMENT 'Course title',
  Status VARCHAR(50) NOT NULL DEFAULT 'Draft' COMMENT 'Draft, Published, Archived',
  IsDeleted BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'Soft delete flag',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Creation timestamp',
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT 'Last update timestamp',
  INDEX idx_status (Status),
  INDEX idx_deleted (IsDeleted),
  INDEX idx_created (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Courses table';

-- Create Lessons table
CREATE TABLE IF NOT EXISTS Lessons (
  Id CHAR(36) PRIMARY KEY COMMENT 'GUID',
  CourseId CHAR(36) NOT NULL COMMENT 'Foreign key to Courses',
  Title VARCHAR(255) NOT NULL COMMENT 'Lesson title',
  `Order` INT NOT NULL COMMENT 'Lesson order in course',
  IsDeleted BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'Soft delete flag',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Creation timestamp',
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT 'Last update timestamp',
  CONSTRAINT fk_lessons_course FOREIGN KEY (CourseId) REFERENCES Courses(Id),
  INDEX idx_course (CourseId),
  INDEX idx_deleted (IsDeleted),
  UNIQUE KEY uk_course_order (CourseId, `Order`, IsDeleted)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Lessons table';

-- Create AspNetUsers table (Identity)
CREATE TABLE IF NOT EXISTS AspNetUsers (
  Id CHAR(36) PRIMARY KEY,
  UserName VARCHAR(256) UNIQUE,
  NormalizedUserName VARCHAR(256) UNIQUE,
  Email VARCHAR(256),
  NormalizedEmail VARCHAR(256) UNIQUE,
  EmailConfirmed BOOLEAN NOT NULL DEFAULT FALSE,
  PasswordHash LONGTEXT,
  SecurityStamp LONGTEXT,
  ConcurrencyStamp LONGTEXT,
  PhoneNumber VARCHAR(20),
  PhoneNumberConfirmed BOOLEAN NOT NULL DEFAULT FALSE,
  TwoFactorEnabled BOOLEAN NOT NULL DEFAULT FALSE,
  LockoutEnd DATETIME(6),
  LockoutEnabled BOOLEAN NOT NULL DEFAULT TRUE,
  AccessFailedCount INT NOT NULL DEFAULT 0,
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  INDEX idx_username (NormalizedUserName),
  INDEX idx_email (NormalizedEmail)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core Identity Users';

-- Create AspNetRoles table
CREATE TABLE IF NOT EXISTS AspNetRoles (
  Id CHAR(36) PRIMARY KEY,
  Name VARCHAR(256),
  NormalizedName VARCHAR(256) UNIQUE,
  ConcurrencyStamp LONGTEXT,
  INDEX idx_name (NormalizedName)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core Identity Roles';

-- Create AspNetUserRoles table
CREATE TABLE IF NOT EXISTS AspNetUserRoles (
  UserId CHAR(36) NOT NULL,
  RoleId CHAR(36) NOT NULL,
  PRIMARY KEY (UserId, RoleId),
  CONSTRAINT fk_user_roles_user FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  CONSTRAINT fk_user_roles_role FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
  INDEX idx_role (RoleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core User Roles mapping';

-- Create AspNetUserClaims table
CREATE TABLE IF NOT EXISTS AspNetUserClaims (
  Id INT PRIMARY KEY AUTO_INCREMENT,
  UserId CHAR(36) NOT NULL,
  ClaimType LONGTEXT,
  ClaimValue LONGTEXT,
  CONSTRAINT fk_user_claims FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  INDEX idx_user (UserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core User Claims';

-- Create AspNetUserLogins table
CREATE TABLE IF NOT EXISTS AspNetUserLogins (
  LoginProvider VARCHAR(128) NOT NULL,
  ProviderKey VARCHAR(128) NOT NULL,
  ProviderDisplayName LONGTEXT,
  UserId CHAR(36) NOT NULL,
  PRIMARY KEY (LoginProvider, ProviderKey),
  CONSTRAINT fk_user_logins FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  INDEX idx_user (UserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core External Logins';

-- Create AspNetUserTokens table
CREATE TABLE IF NOT EXISTS AspNetUserTokens (
  UserId CHAR(36) NOT NULL,
  LoginProvider VARCHAR(128) NOT NULL,
  Name VARCHAR(128) NOT NULL,
  Value LONGTEXT,
  PRIMARY KEY (UserId, LoginProvider, Name),
  CONSTRAINT fk_user_tokens FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core User Tokens';

-- Create AspNetRoleClaims table
CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
  Id INT PRIMARY KEY AUTO_INCREMENT,
  RoleId CHAR(36) NOT NULL,
  ClaimType LONGTEXT,
  ClaimValue LONGTEXT,
  CONSTRAINT fk_role_claims FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
  INDEX idx_role (RoleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='ASP.NET Core Role Claims';

-- Display creation confirmation
SELECT 'Assessment database initialized successfully!' AS Status;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'assessment_db';

