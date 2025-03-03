using System.Threading.Tasks;
using DialogueManagerRuntime;
using Godot;

namespace Theadventureofhink.autoloads;

public partial class CutsceneManager : Node
{
    public bool CutscenePlaying;

    public enum TransitionFocus
    {
        Center,
        Player
    }

    [Signal]
    public delegate void CutsceneStartedEventHandler();

    [Signal]
    public delegate void CutsceneEndedEventHandler();

    private ColorRect _transitionRect;
    private AnimationPlayer _animationPlayer;

    private GameManager _gameManager;
    private CameraManager _cameraManager;

    private TransitionFocus _currentTransitionFocus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _transitionRect = GetNode<ColorRect>("CanvasLayer/TransitionRect");
        _animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/TransitionRect/AnimationPlayer");
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _cameraManager = GetNode<CameraManager>(Singletons.CameraManager);
    }

    public override void _PhysicsProcess(double delta)
    {
        var focus = GetTransitionFocus();
        if (_transitionRect.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("focus_pos", focus);
        }
    }

    public void StartDialogue()
    {
        CutscenePlaying = true;
        EmitSignal(SignalName.CutsceneStarted);
        _gameManager.Player.Disable();
    }

    public void EndDialogue()
    {
        EmitSignal(SignalName.CutsceneEnded);
        _gameManager.Player.Enable();
        CutscenePlaying = false;
    }

    public async Task PlaySingleCharacterLine(string characterName, string dialogue)
    {
        StartDialogue();
        await PlayDialogueCharacterLine(characterName, dialogue);
        EndDialogue();
    }

    public async Task PlayDialogueCharacterLine(string characterName, string dialogue)
    {
        var resourceString = $"~ start\n{characterName}: {dialogue}";
        var dialogueManager = (Node)Engine.GetSingleton("DialogueManager");
        var variant = dialogueManager.Call("create_resource_from_text", resourceString);
        var resource = variant.As<Resource>();
        await PlayDialogueResource(resource, "start");
    }

    public async Task PlayDialogue(string dialogue)
    {
        var resourceString = $"~ start\n{dialogue}";
        var dialogueManager = (Node)Engine.GetSingleton("DialogueManager");
        var variant = dialogueManager.Call("create_resource_from_text", resourceString);
        var resource = variant.As<Resource>();
        await PlayDialogueResource(resource, "start");
    }

    public async Task PlayDialogueResource(Resource resource, string start)
    {
        var dialogueManager = GetNode(Singletons.DialogueManager);

        DialogueManager.ShowDialogueBalloon(resource, start);
        await ToSignal(dialogueManager, "dialogue_ended");

        // Prevent last input to be sent to player.cutscene_ended.emit()
        GetViewport().SetInputAsHandled();
        await ToSignal(GetTree().CreateTimer(0.1), "timeout");
    }

    public async Task PlayFullDialogue(Resource resource, string start, float delay = 0.0f)
    {
        StartDialogue();

        if (delay > 0.0)
        {
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }

        await PlayDialogueResource(resource, start);

        EndDialogue();
    }

    public void TransitionIn(TransitionFocus transitionFocus)
    {
        _currentTransitionFocus = transitionFocus;
        TransitionIn();
    }

    public void TransitionIn()
    {
        GD.Print("TransitionIn");
        _animationPlayer.Play("Transition");
    }

    public Task TransitionOut(TransitionFocus transitionFocus)
    {
        _currentTransitionFocus = transitionFocus;
        return TransitionOut();
    }

    public async Task TransitionOut()
    {
        GD.Print("TransitionOut");
        _animationPlayer.PlayBackwards("Transition");
        await ToSignal(_animationPlayer, "animation_finished");
    }

    private Vector2 GetTransitionFocus()
    {
        if (_cameraManager.Camera != null && _currentTransitionFocus == TransitionFocus.Player)
        {
            return _gameManager.Player.GlobalPosition - _cameraManager.Camera.GlobalPosition;
        }

        return new Vector2(160, 90);
    }
}