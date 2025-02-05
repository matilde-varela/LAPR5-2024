# US 6.5.2

<!-- TOC -->
* [US 6.5.2](#us-652)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
  * [5. References](#5-references)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.5.2:** As a healthcare staff member, I want to see appropriate textures (that is, suitable for use in representing
a hospital or clinic) mapped onto the floor, walls, and so on.

**Acceptance Criteria:**

- **6.5.2.1:** The textures should be suitable for a hospital or clinic ambient.

**Dependencies/References:**

**US 6.5.1:** The 3D model should be created so the textures can be applied.
 

## 3. Analysis

To accomplish this user story, it's necessary to search for textures, suitable for a hospital or a clinic, for the
3D module that will be produced. For that will be search textures for the following places:

  * Floor
  * Walls
  * Door


## 4. Design

### 4.1. Realization

To apply the textures in the necessary places, it's needed to have the correct texture files, that can be found in the
public folder. To load the texture, we will use TextureLoader to load an object 
that come from an image that can be supported by the browser, such as:

  * PNG
  * JPG
  * GIF
  * BMP

The floor will use a single texture, applied across the entire hospital 3D model, with an option to adjust the color 
if needed.

Similarly, the walls will have a unified texture across all surfaces in the hospital model. This texture can also be 
color-adjusted if necessary.

For doors, a specific 3D door model will be used, and the chosen texture will be applied to it accordingly. As well as
the others, the color can be adjusted.


## 5. References

Three.js Texture Page : https://threejs.org/docs/#api/en/textures/Texture