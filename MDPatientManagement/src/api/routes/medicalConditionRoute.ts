import { Router } from 'express';
import { celebrate, Joi } from 'celebrate';
import { Container } from 'typedi';
import config from "../../../config";
import IMedicalConditionController from '../../controllers/IControllers/IMedicalConditionController';
import middlewares from '../middlewares';

const route = Router();

export default (app: Router) => {
  app.use('/medicalCondition', route);

  app.use(function (err, req, res, next) {
    if (err.name === "UnauthorizedError"){
      res.status(401).send("Invalid or missing token.");
    } else {
      next(err);
    }
  });

  const ctrl = Container.get(config.controllers.medicalCondition.name) as IMedicalConditionController;

  route.post('/create',
      middlewares.isAuth,
      middlewares.isAuthz(["Admin"]),
    celebrate({
      body: Joi.object({
        id: Joi.string().required(),
        designation: Joi.string().required(),
        description: Joi.string().required(),
        symptoms: Joi.array().required(),
      })
    }),
    (req, res, next) => ctrl.createMedicalCondition(req, res, next) );

    route.get('/get-all-medical-conditions',
        middlewares.isAuth,
        middlewares.isAuthz(["Doctor", "Admin"]),
        (req, res, next) => ctrl.getAllMedicalCondition(req, res, next) );
  
      route.post('/get-medical-condition-by-id',
        middlewares.isAuth,
        middlewares.isAuthz(["Doctor"]),
        celebrate({
          body: Joi.object({
            id : Joi.string().required()
          }),
        }),
        (req, res, next) => ctrl.getMedicalConditionById(req, res, next) );

};