using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Button))]
[ExecuteInEditMode] // to apply the skill name to the button also in edit mode
public class SkillTreeNode : MonoBehaviour
{
    [SerializeField]
    private Color unlockedColor = Color.white;

    [SerializeField]
    private List<SkillTreeNode> requirements = new();

    // only make requirements available in Inspector, set _nextSkills automatically in Awake()
    /*
    Why not the other way around? Constructing the skill graph top-down and 
    setting the requirements of the current layer feels more natural (to me)
    than constructing the graph bottom-up
    */
    private List<SkillTreeNode> _nextSkills = new();

    private Button skillButton;

    private string _skillName = string.Empty;

    /// <summary>
    /// determines whether you can actually use the skill
    /// </summary>
    private bool _isLocked = true;

    public string GetSkillName() { return _skillName; }
    public bool IsLocked() { return _isLocked; }

    #region SkillTreeBasedOnHierarchy
    /*
    /// <summary>
    /// contains next skills depending on scene hierarchy
    /// </summary>
    private SkillTreeNode[] _nextSkills;

    /// <summary>
    /// contains skill requirements depending on scene hierarchy
    /// </summary>
    private SkillTreeNode[] _requirements;
    */
    #endregion

    private void Awake()
    {
        skillButton = GetComponent<Button>();

        // this way you only have to change the prefab Name
        if(GetSkillName() == string.Empty) _skillName = gameObject.name;

        #region SkillTreeBasedOnHierarchy
        /* --------- small idea to make the skill tree dependent on the scene hierarchy -----------
         * --------- However, isn't applicable for arbitrary graphs 
        // set next skills depending on scene hierarchy
        // the line below is taken from https://discussions.unity.com/t/how-to-get-first-depth-child-components-of-a-parent-object/76169/8
        _nextSkills = gameObject.GetComponentsInChildren<SkillTreeNode>().Where(t => t.GetInstanceID() != this.GetInstanceID() && t.transform.parent == this.transform).ToArray();

        // set previous skills depending on scene hierarchy
        Transform parentParent = transform.parent.parent;
        if (parentParent != null)
        {
            _requirements = parentParent.GetComponentsInChildren<SkillTreeNode>().Where(t => t.GetInstanceID() != parentParent.GetInstanceID() && t.transform.parent == parentParent.transform).ToArray();
        } else
        {
            // if parentParent doesn't exist, there are no requirements as the skill tree is to high in the scene hierarchy
            _requirements = new SkillTreeNode[0];
        }
        */
        #endregion

        foreach(SkillTreeNode skill in requirements)
        {
            skill.AddNextSkill(this);
        }

        // automatically change text on Button to match skillName
        // TMP for text on button
        try
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = GetSkillName();
        } catch {
            Debug.LogError("There should be a TextMeshPro Object below this button");
        }
    }

    private void Start()
    {
        /*
        very weird interaction: 
        if I have this exact line in Awake() it throws me an error at line 63 saying that the skillButton reference is not set
        even though I am literally setting it lines before in line 36
        anyway, to work around that, I put this line in start since the start function is called after awake and here the reference to skillButton seems to work 
        */
        // check if skill can be unlocked; otherwise lock it (also visually)
        CanBeUnlocked();
    }

    /// <summary>
    /// <br> DOES NOT CHECK FOR CYCLIC DEPENDENCIES! </br>
    /// <br> Checks if the skill can be unlocked.</br>
    /// </summary>
    /// <returns>True if the skill can be unlocked</returns>
    public bool CanBeUnlocked() {
        // go through all direct requirements
        foreach (SkillTreeNode skill in requirements)
        {
            // recursively go through their requirements (DOES NOT CHECK FOR CYCLIC DEPENDENCIES!)
            if (!skill.CanBeUnlocked())
            {
                skillButton.interactable = false;
                return false;
            }
        }
        if(IsLocked()) skillButton.interactable = true;
        // return true when we're not locked
        return !IsLocked();
    }

    /// <summary>
    /// This method just unlocks the skill. Changing the button color and interactibility
    /// </summary>
    public void Unlock()
    {
        // unlock skill
        _isLocked = false;
        Debug.Log("The skill " + GetSkillName() + " has been unlocked!");

        // update nextSkills
        foreach (SkillTreeNode skill in _nextSkills)
        {
            skill.CanBeUnlocked();
        }

        // visual:
        // using the workaround below because this doesn't work: skillButton.colors.disabledColor = Color.blue;
        ColorBlock newColorBlock = skillButton.colors;
        newColorBlock.disabledColor = unlockedColor;
        skillButton.colors = newColorBlock;

        // locked for interaction
        skillButton.interactable = false;
    }

    public void AddNextSkill(SkillTreeNode requirement)
    {
        _nextSkills.Add(requirement);
    }
}
