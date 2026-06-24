-- Adds the ROLE column used for JWT role-based authorization.
-- The project is DB-first, so run this once against the WebApiShop database
-- before using the JWT feature.
--
-- Existing rows default to 'User'. Promote an administrator manually, e.g.:
--   UPDATE USERS SET ROLE = 'Admin' WHERE USER_NAME = 'admin@example.com';

ALTER TABLE USERS
    ADD ROLE VARCHAR(20) NOT NULL
    CONSTRAINT DF_USERS_ROLE DEFAULT 'User';
