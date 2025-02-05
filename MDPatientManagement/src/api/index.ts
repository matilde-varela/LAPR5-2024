import { Router } from 'express';
import medicalCondition from './routes/medicalConditionRoute';
import allergy from './routes/allergyRoute';
import medicalRecord from './routes/medicalRecordRoute';

export default () => {
	const app = Router();

	medicalCondition(app);
	allergy(app);
	medicalRecord(app);

	return app
}

