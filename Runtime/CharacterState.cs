using BBUnity.StateMachines;

namespace BBUnity.GameEntities {

    /// <summary>
    /// Subclass of State which provides extra functionality for the
    /// character itself
    /// </summary>
    class CharacterState : State {

        Character _character;
        
        CharacterState(Character character) {

        }
    }
}