using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GymTrainer : MonoBehaviour
{
    [Header("Animator")]
    public Animator TrainerAnimator; // SportyGirl�� Animator
    public Transform avatarTransform;
    //public Animator AvatarAnimator; // SantaClaus�� Animator

    [Header("Exercise")]
    public Button fullCourseButton;
    public Button jumpingJacksButton;
    public Button pushUpButton;
    public Button burpeeButton;
    public Button squatButton;

    [Header("Stop")]
    public Button stopButton;

    [Header("Text")]
    public TextMeshProUGUI exerciseText;
    public TextMeshProUGUI setText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI restText;

    private bool activated = false;
    private Coroutine currentRoutine = null;

    private void Start()
    {
        exerciseText.text = "";
        restText.text = "";

        fullCourseButton.onClick.AddListener(() =>
        {
            if (!activated) StartFullCourse();
        });

        jumpingJacksButton.onClick.AddListener(() =>
        {
            if (!activated) StartJumpingJacks();
        });

        pushUpButton.onClick.AddListener(() =>
        {
            if (!activated) StartPushUp();
        });

        burpeeButton.onClick.AddListener(() =>
        {
            if (!activated) StartBurpee();
        });

        squatButton.onClick.AddListener(() =>
        {
            if (!activated) StartSquat();
        });

        stopButton.onClick.AddListener(StopExercise);
    }

    private void StartFullCourse()
    {
        activated = true;
        // GymTrainerController �ʱ�ȭ �� ����
        currentRoutine = StartCoroutine(ExerciseRoutine());
    }

    private void StartJumpingJacks()
    {
        activated = true;
        // GymTrainerController �ʱ�ȭ �� ����
        currentRoutine = StartCoroutine(JumpingJacksRoutine());
    }

    private void StartPushUp()
    {
        activated = true;
        // GymTrainerController �ʱ�ȭ �� ����
        currentRoutine = StartCoroutine(PushUpRoutine());
    }

    private void StartBurpee()
    {
        activated = true;
        // GymTrainerController �ʱ�ȭ �� ����
        currentRoutine = StartCoroutine(BurpeeRoutine());
    }

    private void StartSquat()
    {
        activated = true;
        // GymTrainerController �ʱ�ȭ �� ����
        currentRoutine = StartCoroutine(SquatRoutine());
    }


    private void StopExercise()
    {
        activated = false;
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;

            Debug.Log("Exercises Canceled!");
        }
        SetAnimatorParameters(0, false);
        exerciseText.text = "";
        setText.text = "0 /3��Ʈ";
        countText.text = "00 /10ȸ";
        restText.text = "";
    }

    private IEnumerator ExerciseRoutine()
    {
        // Idle State���� ����
        SetAnimatorParameters(0, false);

        yield return StartCoroutine(JumpingJacksRoutine());
        yield return StartCoroutine(RestRoutine(60));
        if (!activated) yield break;

        /*yield return StartCoroutine(PushUpRoutine());
        yield return StartCoroutine(RestRoutine(60));
        if (!activated) yield break;*/

        yield return StartCoroutine(BurpeeRoutine());
        yield return StartCoroutine(RestRoutine(60));
        if (!activated) yield break;

        yield return StartCoroutine(SquatRoutine());

        // � ��ƾ ����
        Debug.Log("All exercises completed!");
        activated = false;
        currentRoutine = null;
    }

    private IEnumerator RestRoutine(int duration)
    {
        activated = true;

        for (int sec = duration; sec > 0; sec--)
        {
            if (!activated)
            {
                restText.text = "";
                yield break; // ��ƾ ��� ����
            }
            restText.text = $"�޽� {sec}��";
            yield return new WaitForSecondsRealtime(1f);
        }
        restText.text = "";
    }

    private IEnumerator JumpingJacksRoutine()
    {
        exerciseText.text = "�ȹ����ٱ�";
        // Idle State���� ����
        SetAnimatorParameters(0, false);

        // ó�� 3�� ���
        yield return new WaitForSeconds(3f);

        // 3��Ʈ �ݺ�
        for(int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);
            if (!activated) yield break;

            setText.text = (i+1) + " /3��Ʈ";
            // Jumping Jacks ����
            SetAnimatorParameters(1, true);
            countText.text = "1 /10ȸ";

            // 10ȸ �ݺ�
            for (int j = 1; j < 10; j++)
            {
                if (!activated) yield break;

                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10ȸ";
            }

            // �ݺ� ����
            SetAnimatorParameters(0, false);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }


        // � ����
        Debug.Log("Jumping Jacks completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3��Ʈ";
        countText.text = "00 /10ȸ";

    }

    private IEnumerator PushUpRoutine()
    {
        exerciseText.text = "�ȱ������";

        // Idle State���� ����
        SetAnimatorParameters(0, false);

        // ó�� 3�� ���
        yield return new WaitForSeconds(3f);

        // 3��Ʈ �ݺ�
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 180f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3��Ʈ";

            // Push Up ����
            SetAnimatorParameters(2, true);

            // 10ȸ �ݺ�
            for (int j = 0; j < 10; j++)
            {
                if (!activated) yield break;

                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10ȸ";
            }

            // �ݺ� ����
            SetAnimatorParameters(0, false);

            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
            if (i < 2)
            {
                // 30�� ��� (1�ʸ��� �ؽ�Ʈ ������Ʈ)
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // � ����
        Debug.Log("Pushup completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3��Ʈ";
        countText.text = "00 /10ȸ";
    }

    private IEnumerator BurpeeRoutine()
    {
        exerciseText.text = "����";

        // Idle State���� ����
        SetAnimatorParameters(0, false);

        // ó�� 3�� ���
        yield return new WaitForSeconds(3f);

        // 3��Ʈ �ݺ�
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3��Ʈ";

            // Burpee ����
            SetAnimatorParameters(3, true);
            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
            countText.text = "1 /10ȸ";

            // 10ȸ �ݺ�
            for (int j = 1; j < 10; j++)
            {
                if (!activated) yield break;
                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10ȸ";
            }
            // �ݺ� ����
            SetAnimatorParameters(0, false);
            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // � ����
        Debug.Log("Burpee completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3��Ʈ";
        countText.text = "00 /10ȸ";
    }

    private IEnumerator SquatRoutine()
    {
        exerciseText.text = "����Ʈ";

        // Idle State���� ����
        SetAnimatorParameters(0, false);

        // ó�� 3�� ���
        yield return new WaitForSeconds(3f);

        // 3��Ʈ �ݺ�
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3��Ʈ";

            // squat ����
            SetAnimatorParameters(4, true);

            // 10ȸ �ݺ�
            for (int j = 0; j < 10; j++)
            {
                if (!activated) yield break;
                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10ȸ";
            }

            // �ݺ� ����
            SetAnimatorParameters(0, false);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // � ����
        Debug.Log("Squat completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3��Ʈ";
        countText.text = "00 /10ȸ";
    }

    private void SetAnimatorParameters(int exerciseValue, bool repeat)
    {
        TrainerAnimator.SetInteger("Exercise", exerciseValue);
        TrainerAnimator.SetBool("Repeat", repeat);
       /* AvatarAnimator.SetInteger("Exercise", exerciseValue);
        AvatarAnimator.SetBool("Repeat", repeat);*/
    }
}
