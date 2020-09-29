# Internal-Project-3

ABOUT:

A simple document management system made for professional use for a mock customer (ideagen glasgow) at the end of my third year at glasgow caledonian university. This program will although you to upload documents and keep version control on them.

GETTING STARTED:

Once you download the project you will need to reseed the database by doing update-database -verbose in the packet manager console. Users will have to interact with the default accounts to make the program work for them see the migration file for the default account logins

Installing

Simple download the code and follow the steps in the getting started section to get this working

USAGE:

Admins are the only users who can create new users
Admins can edit users names, passwords and whether their account is archive or not (if your account is archived you cannot sign in till your account is unarchived)
Admins can created new roles
Admins can also give accounts new roles including administrator, distrubutee and document author
All users can view their own documents
Document Authors can create new documents
When document authors edit documents a new version of the document is created
when a document is set to active a new version of it is created
distributees can view documents but cannot interact with them
