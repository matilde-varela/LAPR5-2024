"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = require("express");
const celebrate_1 = require("celebrate");
const typedi_1 = require("typedi");
const config_1 = __importDefault(require("../../../config"));
const middlewares_1 = __importDefault(require("../middlewares"));
const route = (0, express_1.Router)();
exports.default = (app) => {
    app.use('/roles', route);
    app.use(function (err, req, res, next) {
        if (err.name === "UnauthorizedError") {
            res.status(401).send("Invalid or missing token.");
        }
        else {
            next(err);
        }
    });
    const ctrl = typedi_1.Container.get(config_1.default.controllers.role.name);
    route.post('', middlewares_1.default.isAuth, /// YOU GUYS NEED TO ADD THIS
    middlewares_1.default.isAuthz(["Admin"]), /// YOU GUYS NEED TO ADD THIS
    (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            name: celebrate_1.Joi.string().required()
        })
    }), (req, res, next) => ctrl.createRole(req, res, next));
    route.put('', (0, celebrate_1.celebrate)({
        body: celebrate_1.Joi.object({
            id: celebrate_1.Joi.string().required(),
            name: celebrate_1.Joi.string().required()
        }),
    }), (req, res, next) => ctrl.updateRole(req, res, next));
};
//# sourceMappingURL=roleRoute.js.map