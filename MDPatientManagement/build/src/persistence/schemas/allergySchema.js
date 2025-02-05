"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const mongoose_1 = __importDefault(require("mongoose"));
const Allergy = new mongoose_1.default.Schema({
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
}, { timestamps: true });
exports.default = mongoose_1.default.model('Allergy', Allergy);
//# sourceMappingURL=allergySchema.js.map