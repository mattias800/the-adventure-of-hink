using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class JumpPad : Node2D
{
    [Export] public int power = 400;

    private AnimatedSprite2D animated_sprite;
    private Area2D area;

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);

        animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        area = GetNode<Area2D>("Area2D");

        animated_sprite.Play("idle");
    }

    public void OnArea2dAreaShapeEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index)
    {
        if (area.IsInGroup("player_bounce"))
        {
            animated_sprite.Stop();
            animated_sprite.Play("blam");
            animated_sprite.AnimationLooped += on_hit_animation_done;
            // animated_sprite.Connect(AnimatedSprite2D.SignalName.AnimationLooped, Callable.From(on_hit_animation_done));
            _gameManager.Player.TriggerForce(new Vector2(0, -power));
            // active = false
            // await get_tree().create_timer(0.1).timeout
            // active = true
        }
    }

    public void on_hit_animation_done()
    {
        animated_sprite.Play("idle");
        animated_sprite.AnimationFinished -= on_hit_animation_done;
    }
}