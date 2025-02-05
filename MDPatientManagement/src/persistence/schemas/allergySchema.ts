import { IAllergyPersistence } from '../../dataschema/IAllergyPersistence';
import mongoose from 'mongoose';

const Allergy = new mongoose.Schema(
  {
    code: {
      type: String,
      required: [true, 'Please enter code'],
      index: true,
    },

    designation: {
      type: String,
      required: [true, 'Please enter designation'],
      index: true,
    },

    description: {
      type: String,
      index: true,
    },
  },
  { timestamps: true },
);

export default mongoose.model<IAllergyPersistence & mongoose.Document>('Allergy', Allergy);
