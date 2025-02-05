import { Vector2 } from "three";

export default class Orientation extends Vector2 {
    constructor(h: number = 0, v: number = 0) {
        super(h, v); // Pass initial values to the Vector2 constructor
    }

    // Getter for horizontal value (h)
    get h(): number {
        return this.x;
    }

    // Setter for horizontal value (h)
    set h(value: number) {
        this.x = value;
    }

    // Getter for vertical value (v)
    get v(): number {
        return this.y;
    }

    // Setter for vertical value (v)
    set v(value: number) {
        this.y = value;
    }
}
