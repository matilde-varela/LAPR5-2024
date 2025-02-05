import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import IMedicalConditionDTO from "../../../src/dto/IMedicalConditionDTO";
import MedicalConditionController from "../../../src/controllers/medicalConditionController";
import IMedicalConditionService from "../../../src/services/IServices/IMedicalConditionService";
import { Result } from "../../../src/core/logic/Result";
import { Response, Request, NextFunction } from 'express';

describe("MedicalConditionController Unit Tests", function () {
    const sandbox = sinon.createSandbox();
    this.timeout(5000);

    beforeEach(() => {
        Container.reset();

        const mockLogger = {
            info: sandbox.stub(),
            error: sandbox.stub(),
            warn: sandbox.stub(),
            debug: sandbox.stub(),
        };
        Container.set("logger", mockLogger);

        // let medicalConditionSchemaInstance = require("../../../src/persistence/schemas/medicalConditionSchema").default;
        // Container.set("medicalConditionSchema", medicalConditionSchemaInstance);

        // let medicalConditionRepoClass = require("../../../src/repos/medicalConditionRepo").default;
        // let medicalConditionRepoInstance = Container.get(medicalConditionRepoClass);
        // Container.set("MedicalConditionRepo", medicalConditionRepoInstance);

        let medicalConditionServiceClass = require("../../../src/services/medicalConditionService").default;
        let medicalConditionServiceInstance = Container.get(medicalConditionServiceClass);
        Container.set("MedicalConditionService", medicalConditionServiceInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should create a medical condition successfully with medicalConditionService stub", async () => {
        const medicalConditionDTO: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'valid designation',
            description: 'valid description',
            symptoms: ['Symptom 1', 'Symptom 2']
        };

        let body = medicalConditionDTO;
        let req: Partial<Request> = {};
        req.body = body;
        let res: Partial<Response> = {
            json: sinon.spy()
        };
        let next: Partial<NextFunction> = () => { };

        let medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        sinon.stub(medicalConditionServiceInstance, "createMedicalCondition").returns(Result.ok<IMedicalConditionDTO>(medicalConditionDTO));

        const ctrl = new MedicalConditionController(medicalConditionServiceInstance as IMedicalConditionService);
        // Act
        await ctrl.createMedicalCondition(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalConditionDTO));
    });

    it("should return all medical conditions successfully", async () => {
        const mockConditions: IMedicalConditionDTO[] = [
          {
            id: "MC1",
            designation: "Condition 1",
            description: "Description 1",
            symptoms: ["Symptom 1", "Symptom 2"],
          },
        ];
    
        const req: Partial<Request> = {};
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          json: sinon.stub(),
        };
        const next: Partial<NextFunction> = sinon.stub();
    
        const service = Container.get("MedicalConditionService") as IMedicalConditionService;
        sinon.stub(service, "getAllMedicalConditions").resolves(Result.ok(mockConditions));
    
        const controller = new MedicalConditionController(service);
       // Act
        await controller.getAllMedicalCondition(req as Request, res as Response, next as NextFunction);
    
        // Assert
        sinon.assert.calledWith(res.status as sinon.SinonStub, 200);
        sinon.assert.calledWith(res.json as sinon.SinonStub, mockConditions);
      });
    
      it("should return 404 if no medical conditions are found", async () => {
        const req: Partial<Request> = {};
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          send: sinon.stub(),
        };
        const next: Partial<NextFunction> = sinon.stub();
    
        const service = Container.get("MedicalConditionService") as IMedicalConditionService;
        sinon.stub(service, "getAllMedicalConditions").resolves(Result.fail("Medical Conditions not found"));
    
        const controller = new MedicalConditionController(service);
        // Act
        await controller.getAllMedicalCondition(req as Request, res as Response, next as NextFunction);
    
        // Assert
        sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
        sinon.assert.calledOnce(res.send as sinon.SinonStub);
      });
    
      it("should return a medical condition by ID successfully", async () => {
        const mockCondition: IMedicalConditionDTO = {
          id: "MC1",
          designation: "Condition 1",
          description: "Description 1",
          symptoms: ["Symptom 1", "Symptom 2"],
        };
    
        const req: Partial<Request> = {
          body: { id: "MC1" },
        };
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          json: sinon.stub(),
        };
        const next: Partial<NextFunction> = sinon.stub();
    
        const service = Container.get("MedicalConditionService") as IMedicalConditionService;
        sinon.stub(service, "getMedicalConditionById").resolves(Result.ok(mockCondition));
    
        const controller = new MedicalConditionController(service);
        // Act
        await controller.getMedicalConditionById(req as Request, res as Response, next as NextFunction);
    
        // Assert
        sinon.assert.calledWith(res.status as sinon.SinonStub, 200);
        sinon.assert.calledWith(res.json as sinon.SinonStub, mockCondition);
      });
    
      it("should return 404 if medical condition by ID is not found", async () => {
        const req: Partial<Request> = {
          body: { id: "MC1" },
        };
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          send: sinon.stub(),
        };
        const next: Partial<NextFunction> = sinon.stub();
    
        const service = Container.get("MedicalConditionService") as IMedicalConditionService;
        sinon.stub(service, "getMedicalConditionById").resolves(Result.fail("Medical condition not found"));
    
        const controller = new MedicalConditionController(service);
        // Act
        await controller.getMedicalConditionById(req as Request, res as Response, next as NextFunction);
    
        // Assert
        sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
        sinon.assert.calledOnce(res.send as sinon.SinonStub);
      });
    
      it("should return 400 for invalid ID in getMedicalConditionById", async () => {
        const req: Partial<Request> = {
          body: { id: null },
        };
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          json: sinon.stub(),
        };
        const next: Partial<NextFunction> = sinon.stub();
    
        const service = Container.get("MedicalConditionService") as IMedicalConditionService;
    
        const controller = new MedicalConditionController(service);
        // Act
        await controller.getMedicalConditionById(req as Request, res as Response, next as NextFunction);
    
        // Assert
        sinon.assert.calledWith(res.status as sinon.SinonStub, 400);
        sinon.assert.calledWith(res.json as sinon.SinonStub, { error: "Invalid id provided." });
      });
});
