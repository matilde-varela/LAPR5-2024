
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Guard } from "../core/logic/Guard";
import { Result } from "../core/logic/Result";
import { TextUtil } from "../utils/TextUtil";

export class AllergyCode extends UniqueEntityID {

  get code (): String {
    return this.code.toString();
  }

  public constructor (code : string) {
    super(code)
  }

  public static create (code : string): Result<AllergyCode> {
      const guardResult = Guard.againstNullOrUndefined(code, 'code');
      if (!TextUtil.isICD11Code(code)){
        return Result.fail<AllergyCode>("The provided code does not respect the ICD-11 format.");
      }
      if (!guardResult.succeeded) {
        return Result.fail<AllergyCode>(guardResult.message);
      } else {
        return Result.ok<AllergyCode>(new AllergyCode(code))
      }
    }
}