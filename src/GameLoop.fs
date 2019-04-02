﻿module internal Xelmish.XnaCore

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Model
open Viewables

type GameLoop (config: GameConfig) as this = 
    inherit Game ()

    let graphics = new GraphicsDeviceManager (this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let mutable view: (Viewable list) option = None
    let mutable keyboardState = Unchecked.defaultof<KeyboardState>
    let mutable mouseState = Unchecked.defaultof<MouseState>

    let clearColor = Option.map xnaColor config.clearColour
    let defaultBounds = 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight

    do 
        match config.resolution with
        | Windowed (w,h) -> 
            graphics.PreferredBackBufferWidth <- w
            graphics.PreferredBackBufferHeight <- h

        this.IsMouseVisible <- config.mouseVisible
        graphics.SynchronizeWithVerticalRetrace <- true
        this.IsFixedTimeStep <- false 
        
    member __.View
        with set value = 
            view <- value

    override __.LoadContent () = 
        spriteBatch <- new SpriteBatch (this.GraphicsDevice)

    override __.Update _ =
        keyboardState <- Keyboard.GetState ()
        mouseState <- Mouse.GetState ()

    override __.Draw gameTime =

        Option.iter this.GraphicsDevice.Clear clearColor

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)

        match view with
        | None -> __.Exit ()
        | Some v -> 
            let drawState = { 
                gameTime = gameTime
                keyboardState = keyboardState
                mouseState = mouseState 
                spriteBatch = spriteBatch }
            List.iter (renderViewable drawState defaultBounds) v

        spriteBatch.End ()