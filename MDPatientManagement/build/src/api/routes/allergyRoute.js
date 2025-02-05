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
    app.use('/Allergy', route);
    const ctrl = typedi_1.Container.get(config_1.default.controllers.allergy.name);
    route.post('/create-allergy', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            code: celebrate_1.Joi.string().required(),
            designation: celebrate_1.Joi.string().required(),
            description: celebrate_1.Joi.string()
        })
    }), (req, res, next) => ctrl.createAllergy(req, res, next));
    route.patch('/update-allergy', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            code: celebrate_1.Joi.string().required(),
            designation: celebrate_1.Joi.string().required(),
            description: celebrate_1.Joi.string()
        }),
    }), (req, res, next) => ctrl.updateAllergy(req, res, next));
    route.get('/get-all-allergies', (req, res, next) => ctrl.getAllAllergies(req, res, next));
    route.post('/get-allergies-filtered', (req, res, next) => ctrl.getAllergiesByFilter(req, res, next));
    route.get('/get-allergy-by-code', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            code: celebrate_1.Joi.string().required()
        }),
    }), (req, res, next) => ctrl.getAllergyByCode(req, res, next));
};
//# sourceMappingURL=allergyRoute.js.map