"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const mongoose_1 = __importDefault(require("mongoose"));
const MedicalRecordSchema = new mongoose_1.default.Schema({
    domainId: { type: String, unique: true },
    id: { type: String, unique: true, required: true },
    medicalRecordNumber: { type: String, required: true },
    medicalConditions: { type: Array },
    allergies: { type: Array },
    description: { type: String }
}, { timestamps: true });
exports.default = mongoose_1.default.model('Medical_Record', MedicalRecordSchema);
//# sourceMappingURL=medicalRecordSchema.js.map