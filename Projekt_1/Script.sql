create table patient (
	id int primary key generated always as identity,
	first_name text not null,
	last_name text not null,
	OIB text not null,
	date_of_birth date not null,
	gender char not null
);

create table doctor (
	id int primary key generated always as identity,
	first_name text not null,
	last_name text not null
);

create table medical_document (
	id int primary key generated always as identity,
	title text not null,
	sickness text not null,
	diagnosis text not null,
	start_of_sickness date not null,
	end_of_sickness date,
	doctor_id int references doctor(id) not null,
 	patient_id int references patient(id) not null
);

create table recipe (
	id int primary key generated always as identity,
	medicine text not null,
	description text not null,
	start_of_medication date not null,
	end_of_medication date not null,
	doctor_id int references doctor(id) not null,
 	patient_id int references patient(id) not null
);

create table checkup_type (
	id int primary key generated always as identity,
	acronym text,
	name text not null
);

create table checkup (
	id int primary key generated always as identity,
	title text not null,
	description text not null,
	time timestamp default now(),
	type_id int references checkup_type(id) not null,
 	patient_id int references patient(id) not null
);

create table checkup_doctor(
	id int primary key generated always as identity,
	checkup_id int references checkup(id) not null,
	doctor_id int references doctor(id) not null
);

create table checkup_image (
	id int primary key generated always as identity,
	name text,
	img_path text not null,
	checkup_id int references checkup(id) not null
)

insert into patient (first_name, last_name, oib, date_of_birth, gender) values ('Darjan', 'Šprajc', '03591353570', '2003-10-31', 'M');

insert into doctor (first_name, last_name) values ('Borna', 'Skračić');

insert into recipe (medicine, description, start_of_medication, end_of_medication, doctor_id, patient_id) values ('Aspirin C', 'One in the morning, and one in the evening', '3-6-2025', '3-13-2025', 1, 1);

insert into medical_document (title, sickness, diagnosis, start_of_sickness, doctor_id, patient_id) 
values ('Corona virus infected patient', 'Corona Virus 2.0', 'Patient has a temperature of 38, severe headache, has problems with breathing, coughs, eyes are swollen', '3-6-2025', 1, 1);

insert into checkup_type (acronym, name) values ('GP', 'Opći tjelesni pregled');
insert into checkup_type (acronym, name) values ('KRV', 'Test krvi');
insert into checkup_type (acronym, name) values ('X-RAY', 'Rendgensko skeniranje');
insert into checkup_type (acronym, name) values ('CT', 'CT sken');
insert into checkup_type (acronym, name) values ('MR', 'MRI sken');
insert into checkup_type (acronym, name) values ('ULTRA', 'Ultrazvuk');
insert into checkup_type (acronym, name) values ('EKG', 'Elektrokardiogram');
insert into checkup_type (acronym, name) values ('ECHO', 'Ehokardiogram');
insert into checkup_type (acronym, name) values ('EYE', 'Pregled očiju');
insert into checkup_type (acronym, name) values ('DERM', 'Dermatološki pregled');
insert into checkup_type (acronym, name) values ('DENTA', 'Pregled zuba');
insert into checkup_type (acronym, name) values ('MAMMO', 'Mamografija');
insert into checkup_type (acronym, name) values ('NEURO', 'Neurološki pregled');

insert into checkup (title, description, type_id, patient_id) values ('Regulatory checkup', 'Patient seems fine', 1, 1);

insert into checkup_doctor (checkup_id, doctor_id) values (1, 1);

select * from patient
select * from doctor
select * from medical_document
select * from recipe
select * from checkup_type
select * from checkup
select * from checkup_doctor