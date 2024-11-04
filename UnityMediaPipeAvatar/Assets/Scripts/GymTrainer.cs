using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GymTrainer : MonoBehaviour
{
    [Header("Animator")]
    public Animator TrainerAnimator; // SportyGirl의 Animator
    public Transform avatarTransform;
    //public Animator AvatarAnimator; // SantaClaus의 Animator

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
        // GymTrainerController 초기화 및 실행
        currentRoutine = StartCoroutine(ExerciseRoutine());
    }

    private void StartJumpingJacks()
    {
        activated = true;
        // GymTrainerController 초기화 및 실행
        currentRoutine = StartCoroutine(JumpingJacksRoutine());
    }

    private void StartPushUp()
    {
        activated = true;
        // GymTrainerController 초기화 및 실행
        currentRoutine = StartCoroutine(PushUpRoutine());
    }

    private void StartBurpee()
    {
        activated = true;
        // GymTrainerController 초기화 및 실행
        currentRoutine = StartCoroutine(BurpeeRoutine());
    }

    private void StartSquat()
    {
        activated = true;
        // GymTrainerController 초기화 및 실행
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
        setText.text = "0 /3세트";
        countText.text = "00 /10회";
        restText.text = "";
    }

    private IEnumerator ExerciseRoutine()
    {
        // Idle State에서 시작
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

        // 운동 루틴 종료
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
                yield break; // 루틴 즉시 종료
            }
            restText.text = $"휴식 {sec}초";
            yield return new WaitForSecondsRealtime(1f);
        }
        restText.text = "";
    }

    private IEnumerator JumpingJacksRoutine()
    {
        exerciseText.text = "팔벌려뛰기";
        // Idle State에서 시작
        SetAnimatorParameters(0, false);

        // 처음 3초 대기
        yield return new WaitForSeconds(3f);

        // 3세트 반복
        for(int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);
            if (!activated) yield break;

            setText.text = (i+1) + " /3세트";
            // Jumping Jacks 시작
            SetAnimatorParameters(1, true);
            countText.text = "1 /10회";

            // 10회 반복
            for (int j = 1; j < 10; j++)
            {
                if (!activated) yield break;

                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10회";
            }

            // 반복 종료
            SetAnimatorParameters(0, false);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }


        // 운동 종료
        Debug.Log("Jumping Jacks completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3세트";
        countText.text = "00 /10회";

    }

    private IEnumerator PushUpRoutine()
    {
        exerciseText.text = "팔굽혀펴기";

        // Idle State에서 시작
        SetAnimatorParameters(0, false);

        // 처음 3초 대기
        yield return new WaitForSeconds(3f);

        // 3세트 반복
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 180f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3세트";

            // Push Up 시작
            SetAnimatorParameters(2, true);

            // 10회 반복
            for (int j = 0; j < 10; j++)
            {
                if (!activated) yield break;

                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10회";
            }

            // 반복 종료
            SetAnimatorParameters(0, false);

            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
            if (i < 2)
            {
                // 30초 대기 (1초마다 텍스트 업데이트)
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // 운동 종료
        Debug.Log("Pushup completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3세트";
        countText.text = "00 /10회";
    }

    private IEnumerator BurpeeRoutine()
    {
        exerciseText.text = "버피";

        // Idle State에서 시작
        SetAnimatorParameters(0, false);

        // 처음 3초 대기
        yield return new WaitForSeconds(3f);

        // 3세트 반복
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3세트";

            // Burpee 시작
            SetAnimatorParameters(3, true);
            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
            countText.text = "1 /10회";

            // 10회 반복
            for (int j = 1; j < 10; j++)
            {
                if (!activated) yield break;
                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10회";
            }
            // 반복 종료
            SetAnimatorParameters(0, false);
            yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // 운동 종료
        Debug.Log("Burpee completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3세트";
        countText.text = "00 /10회";
    }

    private IEnumerator SquatRoutine()
    {
        exerciseText.text = "스쿼트";

        // Idle State에서 시작
        SetAnimatorParameters(0, false);

        // 처음 3초 대기
        yield return new WaitForSeconds(3f);

        // 3세트 반복
        for (int i = 0; i < 3; i++)
        {
            avatarTransform.position = new Vector3(-2.0f, -6.85f, -8.25f);
            avatarTransform.rotation = Quaternion.Euler(0f, 162f, 0f);

            if (!activated) yield break;
            setText.text = (i + 1) + " /3세트";

            // squat 시작
            SetAnimatorParameters(4, true);

            // 10회 반복
            for (int j = 0; j < 10; j++)
            {
                if (!activated) yield break;
                yield return new WaitForSeconds(TrainerAnimator.GetCurrentAnimatorStateInfo(0).length);
                countText.text = (j + 1) + " /10회";
            }

            // 반복 종료
            SetAnimatorParameters(0, false);

            if (i < 2)
            {
                yield return StartCoroutine(RestRoutine(30));
            }
        }

        // 운동 종료
        Debug.Log("Squat completed!");
        activated = false;
        currentRoutine = null;
        exerciseText.text = "";
        setText.text = "0 /3세트";
        countText.text = "00 /10회";
    }

    private void SetAnimatorParameters(int exerciseValue, bool repeat)
    {
        TrainerAnimator.SetInteger("Exercise", exerciseValue);
        TrainerAnimator.SetBool("Repeat", repeat);
       /* AvatarAnimator.SetInteger("Exercise", exerciseValue);
        AvatarAnimator.SetBool("Repeat", repeat);*/
    }
}
