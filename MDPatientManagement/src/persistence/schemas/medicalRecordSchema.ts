import mongoose from "mongoose";
import { IMedicalRecordPersistence } from "../../dataschema/IMedicalRecordPersistence";


const MedicalRecordSchema = new mongoose.Schema(
    {
        domainId: { type: String, unique: true },
        id: { type: String, unique: true, required: true },
        medicalRecordNumber: { type: String, required: true },
        medicalConditions: { type: Array },
        allergies: { type: Array },
        description: { type: String },
    },
    { timestamps: true },
);


export default mongoose.model<IMedicalRecordPersistence & mongoose.Document>('Medical_Record', MedicalRecordSchema);