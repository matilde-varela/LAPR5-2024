"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = require("express");
const config_1 = __importDefault(require("../../../config"));
const typedi_1 = require("typedi");
const celebrate_1 = require("celebrate");
const route = (0, express_1.Router)();
exports.default = (app) => {
    app.use('/medicalRecord', route);
    const ctrl = typedi_1.Container.get(config_1.default.controllers.medicalRecord.name);
    route.post('/create', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            id: celebrate_1.Joi.string().required(),
            medicalRecordNumber: celebrate_1.Joi.string().required(),
            medicalConditions: celebrate_1.Joi.array().required(),
            allergies: celebrate_1.Joi.array().required(),
            description: celebrate_1.Joi.string(),
        })
    }), (req, res, next) => ctrl.createMedicalRecord(req, res, next));
    route.patch('/update', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            id: celebrate_1.Joi.string().required(),
            medicalRecordNumber: celebrate_1.Joi.string().required(),
            medicalConditions: celebrate_1.Joi.array(),
            allergies: celebrate_1.Joi.array(),
            description: celebrate_1.Joi.string(),
        })
    }), (req, res, next) => ctrl.updateMedicalRecord(req, res, next));
    route.get('/get-all-medical-records', (req, res, next) => ctrl.getAllMedicalRecords(req, res, next));
    route.post('/get-filtered-medical-records', (req, res, next) => ctrl.getFilteredMedicalRecords(req, res, next));
};
//# sourceMappingURL=medicalRecordRoute.js.map