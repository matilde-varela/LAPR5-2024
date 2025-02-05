"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = require("express");
const celebrate_1 = require("celebrate");
const typedi_1 = require("typedi");
const config_1 = __importDefault(require("../../../config"));
const route = (0, express_1.Router)();
exports.default = (app) => {
    app.use('/medicalCondition', route);
    const ctrl = typedi_1.Container.get(config_1.default.controllers.medicalCondition.name);
    route.post('/create', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            id: celebrate_1.Joi.string().required(),
            designation: celebrate_1.Joi.string().required(),
            description: celebrate_1.Joi.string().required(),
            symptoms: celebrate_1.Joi.array().required(),
        })
    }), (req, res, next) => ctrl.createMedicalCondition(req, res, next));
    route.get('/get-all-medical-conditions', (req, res, next) => ctrl.getAllMedicalCondition(req, res, next));
    route.get('/get-medical-condition-by-id', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            id: celebrate_1.Joi.string().required()
        }),
    }), (req, res, next) => ctrl.getAllMedicalCondition(req, res, next));
};
//# sourceMappingURL=medicalConditionRoute.js.map