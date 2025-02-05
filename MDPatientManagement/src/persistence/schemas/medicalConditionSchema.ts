
import mongoose from 'mongoose';
import { IMedicalConditionPersistence } from '../../dataschema/IMedicalConditionPersistence';

const MedicalConditionSchema = new mongoose.Schema(
  {
    domainId: { type: String, unique: true },
    id: { type: String, unique: true },
    designation: { type: String, unique: true},
    description: { type: String },
    symptoms: { type: Array }
  },
  {
    timestamps: true
  }
);

export default mongoose.model<IMedicalConditionPersistence & mongoose.Document>('Medical_Condition', MedicalConditionSchema);
