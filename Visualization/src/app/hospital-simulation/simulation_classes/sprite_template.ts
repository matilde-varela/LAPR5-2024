import * as THREE from "three";

interface TextSpriteParameters {
    fontsize?: number;
    fontface?: string;
    borderThickness?: number;
    borderColor?: { r: number; g: number; b: number; a: number };
    backgroundColor?: { r: number; g: number; b: number; a: number };
}

export default class Sprite {
    object: THREE.Group;
    text: string;
    available : boolean;

    constructor(roomID : string, occupied : boolean) {
        this.object = new THREE.Group();
        this.text = roomID;
        this.available = occupied;

        

        let spritey = this.makeTextSprite(this.text, {
            fontsize: 24,
            fontface: "Georgia",
            borderColor: this.getBorderColorRegardingOccupancy(),
            backgroundColor: this.getBaseColorRegardingOccupancy()
        } );
        spritey.name = "Sprite"

        this.object.add(spritey);
    }

    getBorderColorRegardingOccupancy() : any{
        if (this.available) {
            return { r: 0, g: 255, b: 0, a: 1.0 };
        }
        return { r: 255, g: 0, b: 0, a: 1.0 };
    }

    getBaseColorRegardingOccupancy() : any{
        if (this.available) {
            return { r: 100, g: 255, b: 100, a: 0.8 };
        }
        return { r: 255, g: 100, b: 100, a: 0.8 };
    }

    makeTextSprite(message: string, parameters: TextSpriteParameters = {}): THREE.Sprite {
        const fontface = parameters.fontface || "Arial";
        const fontsize = parameters.fontsize || 18;
        const borderThickness = parameters.borderThickness || 4;
        const borderColor = parameters.borderColor || { r: 0, g: 0, b: 0, a: 1.0 };
        const backgroundColor = parameters.backgroundColor || { r: 255, g: 255, b: 255, a: 1.0 };

        // Create a canvas element to draw the text and background
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d')!;
        context.font = `Bold ${fontsize}px ${fontface}`;

        // Measure the text width
        const metrics = context.measureText(message);
        const textWidth = metrics.width;

        // Draw the background color
        context.fillStyle = `rgba(${backgroundColor.r},${backgroundColor.g},${backgroundColor.b},${backgroundColor.a})`;

        // Draw the border color
        context.strokeStyle = `rgba(${borderColor.r},${borderColor.g},${borderColor.b},${borderColor.a})`;
        context.lineWidth = borderThickness;

        // Draw rounded rectangle with text
        this.roundRect(context, borderThickness / 2, borderThickness / 2, textWidth + borderThickness, fontsize * 1.4 + borderThickness, 6);

        // Set text color and draw text
        context.fillStyle = "rgba(0, 0, 0, 1.0)";
        context.fillText(message, borderThickness, fontsize + borderThickness);

        // Create a texture from the canvas
        const texture = new THREE.Texture(canvas);
        texture.needsUpdate = true;

        // Create sprite material with the texture
        const spriteMaterial = new THREE.SpriteMaterial({ map: texture });
        const sprite = new THREE.Sprite(spriteMaterial);
        sprite.scale.set(2.5, 2.5, 2.5);
        return sprite;
    }

    updateColor(): void {
        const borderColor = this.getBorderColorRegardingOccupancy();
        const backgroundColor = this.getBaseColorRegardingOccupancy();
    
        // Get the text from the sprite
        const textSprite = this.object.getObjectByName("Sprite") as THREE.Sprite;
        if (!textSprite || !(textSprite.material instanceof THREE.SpriteMaterial)) {
            console.warn("Text sprite or its material is not available.");
            return;
        }
    
        // Create a new canvas and redraw the texture
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d')!;
        context.font = "Bold 24px Georgia";
    
        // Measure text width
        const metrics = context.measureText(this.text);
        const textWidth = metrics.width;
    
        const borderThickness = 4; // Thickness of the border
        const fontSize = 24; // Font size for the text
    
        // Draw background with rounded corners
        context.fillStyle = `rgba(${backgroundColor.r},${backgroundColor.g},${backgroundColor.b},${backgroundColor.a})`;
        context.strokeStyle = `rgba(${borderColor.r},${borderColor.g},${borderColor.b},${borderColor.a})`;
        context.lineWidth = borderThickness;
    
        this.roundRect(
            context,
            borderThickness / 2,
            borderThickness / 2,
            textWidth + borderThickness,
            fontSize * 1.4 + borderThickness,
            6 // Corner radius
        );
    
        // Draw text
        context.fillStyle = "rgba(0, 0, 0, 1.0)";
        context.fillText(this.text, borderThickness, fontSize + borderThickness);
    
        // Create a new texture from the canvas
        const newTexture = new THREE.Texture(canvas);
        newTexture.needsUpdate = true;
    
        // Update the sprite material's texture
        textSprite.material.map = newTexture;
        textSprite.material.needsUpdate = true;
        }
    
    

    // Function to draw rounded rectangles
    roundRect(ctx: CanvasRenderingContext2D, x: number, y: number, w: number, h: number, r: number): void {
        ctx.beginPath();
        ctx.moveTo(x + r, y);
        ctx.lineTo(x + w - r, y);
        ctx.quadraticCurveTo(x + w, y, x + w, y + r);
        ctx.lineTo(x + w, y + h - r);
        ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
        ctx.lineTo(x + r, y + h);
        ctx.quadraticCurveTo(x, y + h, x, y + h - r);
        ctx.lineTo(x, y + r);
        ctx.quadraticCurveTo(x, y, x + r, y);
        ctx.closePath();
        ctx.fill();
        ctx.stroke();
    }
}
