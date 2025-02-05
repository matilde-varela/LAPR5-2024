import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MenuComponent } from './components/menu/menu.component';
import { LoginComponent } from './components/login/login.component';
import { AdminComponent } from './components/admin/admin.component';
import { StaffComponent } from './components/staff/staff.component';
import { PatientComponent } from './components/patient/patient.component';
import { CreatePatientProfileComponent } from './components/admin/create-patient-profile/create-patient-profile.component';
import { CreateStaffProfileComponent } from './components/admin/create-staff-profile/create-staff-profile.component';
import { UpdateProfileComponent } from './components/patient/update-profile/update-profile.component';
import { DoctorComponent } from './components/staff/doctor/doctor.component';
import { ListOperationRequestComponent } from './components/staff/doctor/list-operation-request/list-operation-request.component';
import { EditPatientProfileComponent } from './components/admin/edit-patient-profile/edit-patient-profile.component';
import { CreateOperationRequestComponent } from './components/staff/doctor/create-operation-request/create-operation-request.component';
import { HospitalSimulationComponent } from './hospital-simulation/hospital-simulation.component';
import { CreateOperationTypeComponent } from './components/admin/create-operation-type/create-operation-type.component';
import { RegistrationComponent } from './components/patient/registration/registration.component';
import { ActivationComponent } from './components/patient/verify/verify.component';
import { CreateStaffUserComponent } from './components/admin/create-staff-user/create-staff-user.component';
import { VerifyStaffComponent } from './components/staff/verify-staff/verify-staff.component';
import { ListOperationTypeComponent } from './components/admin/list-operation-type/list-operation-type.component';
import { ListStaffProfiles } from './components/admin/list-staff-profiles/list-staff-profiles.component';
import { ListPatientProfiles } from './components/admin/list-patient-profiles/list-patient-profiles.component';
import { EditStaffProfileComponent } from './components/admin/edit-staff-profile/edit-staff-profile.component';
import { VerifyProfileEditComponent } from './components/staff/verify-profile-edit/verify-profile-edit.component';
import { PatientPreferences } from './components/patient/patient-preferences/patient-preferences.component';
import { PatientAccountDeletionConfirm } from './components/patient/confirm-patient-account-deletion/confirm-patient-account-deletion.component';
import { LoginCallbackComponent } from './components/login-callback/login-callback.component';
import { AddOperationRequestPatientComponent } from './components/staff/doctor/add-operation-request-patient/add-operation-request-patient.component';
import { OperationRequestScheduler } from './components/admin/operation-request-scheduler/operation-request-scheduler.component';
import { RemoveOperationRequestPatientComponent } from './components/staff/doctor/remove-operation-request-patient/remove-operation-request-patient.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { UpdatePasswordComponent } from './components/update-password/update-password.component';
import { AddTimeSlotsComponent } from './components/staff/add-time-slots/add-time-slots.component';
import { CreateAllergyComponent } from './components/admin/create-allergy/create-allergy.component';
import { EditAllergyComponent } from './components/admin/edit-allergy/edit-allergy.component';
import { CreateMedicalConditionComponent } from './components/admin/create-medical-condition/create-medical-condition.component';
import { CreateRoomTypeComponent } from './components/admin/create-room-type/create-room-type.component';
import { CreateSpecializationComponent } from './components/admin/create-specialization/create-specialization.component';
import { CreateSurgeryAppointmentComponent } from './components/staff/doctor/create-surgery-appointment/create-surgery-appointment.component';
import { ListAllergies } from './components/staff/doctor/list-allergies/list-allergies.component';
import { ListSpecializationComponent } from './components/admin/list-specialization/list-specialization.component';
import { CreateRoomComponent } from './components/admin/create-room/create-room.component';
import { PrivacyPolicyComponent } from './components/privacy-policy/privacy-policy.component';
import { ListAppointmentComponent } from './components/staff/doctor/list-appointment/list-appointment.component';

import { ListPatientProfilesWithMedicalRecord } from './components/staff/doctor/list-patient-profiles-with-medical-records/list-patient-profiles-with-medical-records.component';
import { ExportMedicalRecordComponent } from './components/patient/export-medical-record/export-medical-record.component';

export const routes: Routes = [
  { path: '', component: LoginComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'login-callback', component: LoginCallbackComponent },
  { path: 'menu', component: MenuComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'Update-UserPassword', component: UpdatePasswordComponent },
  { path: 'menu', component: MenuComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'create-patient-profile', component: CreatePatientProfileComponent },
  { path: 'edit-patient-profile', component: EditPatientProfileComponent },
  { path: 'create-staff-profile', component: CreateStaffProfileComponent },
  { path: 'edit-staff-profile', component: EditStaffProfileComponent },
  { path: 'verify-profile-edit', component: VerifyProfileEditComponent },
  { path: 'staff', component: StaffComponent },
  { path: 'patient/registration', component: RegistrationComponent },
  { path: 'patient/verify', component: ActivationComponent },
  { path: 'staff/verify-staff', component: VerifyStaffComponent },
  { path: 'doctor', component: DoctorComponent },
  { path: 'list-operation-request', component: ListOperationRequestComponent },
  { path: 'patient', component: PatientComponent },
  { path: 'update-patient-profile', component: UpdateProfileComponent },
  { path: 'add-operation-request-patient', component: AddOperationRequestPatientComponent},
  { path: 'remove-operation-request-patient', component: RemoveOperationRequestPatientComponent},
  { path: 'create-operation-request', component: CreateOperationRequestComponent },
  { path: 'list-operation-type', component: ListOperationTypeComponent },
  { path: 'hospital-simulation', component: HospitalSimulationComponent },
  { path: 'create-operation-type', component: CreateOperationTypeComponent },
  { path: 'list-staff-profiles', component: ListStaffProfiles },
  { path: 'list-patient-profiles', component: ListPatientProfiles },
  { path: 'create-staff-user', component: CreateStaffUserComponent },
  { path: 'patient-preferences', component: PatientPreferences },
  { path: 'add-time-slots', component: AddTimeSlotsComponent },
  { path: 'confirm-patient-account-deletion', component: PatientAccountDeletionConfirm },
  { path: 'operation-request-scheduler', component: OperationRequestScheduler },
  { path: 'create-allergy', component: CreateAllergyComponent },
  { path: 'edit-allergy', component: EditAllergyComponent },
  { path: 'create-medical-condition', component: CreateMedicalConditionComponent },
  { path: 'create-room-type', component: CreateRoomTypeComponent },
  { path: 'create-specialization', component: CreateSpecializationComponent},
  { path: 'create-surgery-appointment', component: CreateSurgeryAppointmentComponent},
  { path: 'list-allergies', component: ListAllergies },
  { path: 'list-specialization', component: ListSpecializationComponent},
  { path: 'create-room', component: CreateRoomComponent},
  { path: 'privacy-policy', component: PrivacyPolicyComponent},
  { path: 'list-patient-profiles-with-medical-records', component: ListPatientProfilesWithMedicalRecord},
  { path: 'list-appointment', component: ListAppointmentComponent},
  { path: 'export-medical-record', component: ExportMedicalRecordComponent},
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],  // Use HttpClientModule here
  exports: [RouterModule],
})
export class AppRoutingModule { }
