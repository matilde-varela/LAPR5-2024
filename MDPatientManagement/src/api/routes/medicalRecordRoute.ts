import { Router } from "express";
import IMedicalRecordController from "../../controllers/IControllers/IMedicalRecordController";
import config from "../../../config";
import { Container } from 'typedi';
import { celebrate, Joi } from "celebrate";
import middlewares from "../middlewares";


const route = Router();

export default (app: Router) => {
    app.use('/medicalRecord', route);

    app.use(function (err, req, res, next) {
        if (err.name === "UnauthorizedError"){
          res.status(401).send("Invalid or missing token.");
        } else {
          next(err);
        }
      });

    const ctrl = Container.get(config.controllers.medicalRecord.name) as IMedicalRecordController;

    route.post('/create',
          middlewares.isAuth,
          middlewares.isAuthz(["Admin"]),
        celebrate({
            body: Joi.object({
                id: Joi.string().required(),
                medicalRecordNumber: Joi.string().required(),
                medicalConditions: Joi.array(),
                allergies: Joi.array(),
                description: Joi.string(),
            })
        }),
        (req, res, next) => ctrl.createMedicalRecord(req, res, next));

    route.patch('/update',
        middlewares.isAuth,
        middlewares.isAuthz(["Doctor"]),
        celebrate({
            body: Joi.object({
                id: Joi.string().required(),
                medicalRecordNumber: Joi.string().required(),
                medicalConditions: Joi.array(),
                allergies: Joi.array(),
                description: Joi.string(),
            })
        }),
        (req, res, next) => ctrl.updateMedicalRecord(req, res, next));
    
    route.get('/get-all-medical-records',
        middlewares.isAuth,
        middlewares.isAuthz(["Doctor"]),
        (req, res, next) => ctrl.getAllMedicalRecords(req, res, next));

    route.post('/get-filtered-medical-records',
        middlewares.isAuth,
        middlewares.isAuthz(["Doctor"]),
        (req, res, next) => ctrl.getFilteredMedicalRecords(req, res, next));

    route.post('/export',
        (req, res, next) => ctrl.exportMedicalRecords(req, res, next));

};