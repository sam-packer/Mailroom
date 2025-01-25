create table UnknownPackages
(
    UnknownPackageId int auto_increment
        primary key,
    FullName         text         null,
    Carrier          varchar(25)  null,
    DeliveredDate    varchar(255) null
);

create table Users
(
    UserId     int auto_increment
        primary key,
    First_Name varchar(100) null,
    Last_Name  varchar(100) null,
    email      varchar(255) null,
    password   varchar(255) null,
    building   varchar(100) null,
    unit       int          null,
    Role       varchar(5)   null,
    constraint Users_email_uindex
        unique (email) comment 'Requires emails to be unique'
);

create table Packages
(
    PackageId     int auto_increment
        primary key,
    UserId        int          null,
    Carrier       varchar(50)  null,
    DeliveredDate varchar(255) null,
    PickedUpDate  varchar(255) null,
    Delivered     tinyint(1)   null,
    constraint Packages_Users_UserId_fk
        foreign key (UserId) references Users (UserId)
            on delete cascade
);

create index Packages_Delivered_DeliveredDate_index
    on Packages (Delivered, DeliveredDate)
    comment 'Used on the admin side for all packages not picked up';

create index Packages_Delivered_UserId_index
    on Packages (Delivered, UserId)
    comment 'Used for the resident side';

create index Packages_UserId_DeliveredDate_index
    on Packages (UserId asc, DeliveredDate desc)
    comment 'Used for the user detail page';

create index Packages_UserId_index
    on Packages (UserId)
    comment 'Foreign key index for UserId';

create index Users_Role_index
    on Users (Role)
    comment 'Keep track of roles to ensure the last admin can''t delete themselves';