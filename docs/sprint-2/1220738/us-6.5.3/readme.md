# US 6.5.3

<!-- TOC -->
- [US 6.5.3](#us-653)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
      - [Logical View](#logical-view)
      - [Process View](#process-view)
        - [Level 1](#level-1)
        - [Level 2](#level-2)
        - [Level 3](#level-3)
      - [Development View](#development-view)
      - [Physical View](#physical-view)
<!-- TOC -->

## 1. Context

This is the first time this US is being worked on.
This US pertains to the Admin.

## 2. Requirements

**US 6.5.3:** As a healthcare staff member, I want to see the hospital/clinic floor illuminated with ambient and directional light.

**Acceptance Criteria:**

**US 6.5.3.1 -** The solution should be done in three.js.

**US 6.5.3.2 -** The solution should have ambient light.

**US 6.5.3.3 -** The solution should have directional light.

**Dependencies/References:**

**US 6.5.1:** This US will be dependent on US 6.5.1 as it needs the model for the hospital.

**US 6.5.2:** This US will be dependent on US 6.5.2 as it needs the texture.

**US 6.5.4:** This US will be dependent on US 6.5.4 as it needs the ability to move around to see the effects of light on the model.

**Client Clarifications:**

> None yet.

## 3. Analysis

In User Story 6.5.3, the goal is to create a visual experience of a hospital or clinic floor plan in Three.js with both ambient and directional lighting. This visual feedback should simulate natural and artificial light sources to enhance clarity and depth perception for healthcare staff viewing the space.

### Environmental Setup

The hospital floor will be constructed in a Three.js scene thanks to US 6.5.1, representing the physical layout and essential structures of the space. This includes walls, floors, and possibly furniture or equipment as simplified 3D objects.
Ensuring the scene scale matches realistic dimensions will allow for accurate light distribution and realistic shadows.

### Lighting Types

>***Ambient Light:*** This light will provide a base level of illumination across the entire scene, reducing harsh contrasts. The ambient light intensity should be set to a low-medium level to prevent the environment from looking overly flat.

>***Directional Light:*** This type of light mimics sunlight or primary artificial light sources. Positioning and angling this light will cast realistic shadows, giving the floor and structures a sense of depth and realism.

## 4. Design

### 4.1 Design for Ambient Light

For the ambient light, the idea would be to set to a low-medium level to illuminate the models and the hospital but not to make it too strong as to avoid making the models and textures completely flat.

The color will be a very light yellow as it does a great job at illuminating the scene but not being a harsh color that would "overcolor" the scene, effectively taking away from textures, shadows and models that might depend on a softer, clearer color as a base.

The intensity will be set at 1 by default but parameters such as color and intensity will be adjustable using the **lil-gui** that will be created.

With this design in mind, the ambient light is going to do the job to make shadows as not harsh and provide a general level of base light.

### 4.2 Design for Directional Light

For the directional light we have to think of 3 parameters:

- Color
- Intensity
- Position

As mentioned before, these parameters will be able to be adjusted with the help of the **lil-gui**.

For the **choice of color** we should use colors that are good at illuminating rooms and that can't confuse doctors, nurses or even patients, especially when talking about possible medical emergencies where having a good, stable, neutral light is essential. For that purpose the surgeru rooms will have a white, neutral light, and the normal rooms will have soft yellow light.

For the **intensity**, it will be set by default at 2.5 as it will provide clear, strong light.

And finally for **position**, the lights will be by default set at the center of the "roof" of each room, though some rooms may have two like surgical rooms where light from surgical equipment may also be present and should be accounted for.

## 5. Implementation

// TO BE DONE
