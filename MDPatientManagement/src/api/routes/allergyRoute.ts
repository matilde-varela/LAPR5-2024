import { Router } from 'express';
import { celebrate, Joi } from 'celebrate';

import { Container } from 'typedi';
import IAllergyController from '../../controllers/IControllers/IAllergyController';

import config from "../../../config";
import middlewares from '../middlewares';

const route = Router();

export default (app: Router) => {
  app.use('/Allergy', route);

  app.use(function (err, req, res, next) {
    if (err.name === "UnauthorizedError"){
      res.status(401).send("Invalid or missing token.");
    } else {
      next(err);
    }
  });

  const ctrl = Container.get(config.controllers.allergy.name) as IAllergyController;

  route.post('/create-allergy',
    middlewares.isAuth,
    middlewares.isAuthz(["Admin"]),
    celebrate({
      body: Joi.object({
        code : Joi.string().required(),
        designation : Joi.string().required(),
        description : Joi.string()
      })
    }),
    (req, res, next) => ctrl.createAllergy(req, res, next) );

  route.patch('/update-allergy',
    middlewares.isAuth,
    middlewares.isAuthz(["Admin"]),
    celebrate({
      body: Joi.object({
        code: Joi.string().required(),
        designation : Joi.string().required(),
        description: Joi.string()
      }),
    }),
    (req, res, next) => ctrl.updateAllergy(req, res, next) );

    route.get('/get-all-allergies',
      middlewares.isAuth,
      middlewares.isAuthz(["Doctor", "Admin"]),
      (req, res, next) => ctrl.getAllAllergies(req, res, next) );

    route.post('/get-allergies-filtered',
      middlewares.isAuth,
      middlewares.isAuthz(["Doctor"]),
      (req, res, next) => ctrl.getAllergiesByFilter(req, res, next) );

    route.post('/get-allergy-by-code',
      celebrate({
        body: Joi.object({
          code: Joi.string().required()
        }),
      }),
      (req, res, next) => ctrl.getAllergyByCode(req, res, next) );
};