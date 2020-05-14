using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class GameManager : MonoBehaviour
{
    private bool blink, call, check;
    public Question[] questions;
    private static List<Question> unansweredQuestions;

    private float timeStart;

    private Question currentQuestion;

    [SerializeField]
    private Text factText;

    [SerializeField]
    private Text trueAnswerText;
    [SerializeField]
    private Text falseAnswerText;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float timeBetweenQuestions = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Thread thread = new Thread(CallUDP);
        //Debug.Log("Thread is created");

        try
        {
            thread.Start();
            Debug.Log("Thread started");
            check = true;
        }
        catch (System.Exception)
        {
            ////Debug.Log("Error occured");
        }

        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<Question>();
        }
        SetCurrentQuestion();
    }

    void Update()
    {
       
        if (check)
        {
            timeStart += Time.deltaTime;
            if (timeStart >= 2f)
            {
                check = false;
                timeStart = 0f;
            }
            //Debug.Log(blink);
        }

        if (blink)
        {
            if (!check)
            {
                animator.SetTrigger("True");
                UserSelectTrue();
                StartCoroutine(TransitionToNextQuestion());
            }
        }
        else
        {
            if (!check)
            {
                animator.SetTrigger("False");
                UserSelectFalse();
                StartCoroutine(TransitionToNextQuestion());
            }
        }
    }

    void SetCurrentQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomQuestionIndex];

        factText.text = currentQuestion.fact;

        if (currentQuestion.isTrue)
        {
            trueAnswerText.text = "CORRECT";
            falseAnswerText.text = "WRONG";
        }
        else
        {
            trueAnswerText.text = "WRONG";
            falseAnswerText.text = "CORRECT";
        }
    }

    IEnumerator TransitionToNextQuestion()
    {
        unansweredQuestions.Remove(currentQuestion);

        yield return new WaitForSeconds(timeBetweenQuestions);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        
    }

    public void UserSelectTrue()
    {
        //animator.SetTrigger("True");
        if (currentQuestion.isTrue)
        {
            Debug.Log("Correct");

        }
        else
        {
            Debug.Log("Wrong");
        }

        //StartCoroutine(TransitionToNextQuestion());
        unansweredQuestions.Remove(currentQuestion);
    }

    public void UserSelectFalse()
    {
        //animator.SetTrigger("False");
        if (!currentQuestion.isTrue)
        {
            Debug.Log("Correct");

        }
        else
        {
            Debug.Log("Wrong");
        }

        //StartCoroutine(TransitionToNextQuestion());
        unansweredQuestions.Remove(currentQuestion);
        
    }

    private void CallUDP()
    {
        
        call = true;
        //Creates a UdpClient for reading incoming data.
        UdpClient receivingUdpClient = new UdpClient(5006);

        //Creates an IPEndPoint to record the IP Address and port number of the sender.
        // The IPEndPoint will allow you to read datagrams sent from any source.
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //Debug.Log("RemoteIpEndPoint created");
        try
        {
            while (true)
            {
                // Blocks until a message returns on this socket from a remote host.
                System.Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.Default.GetString(receiveBytes);
                if (receiveBytes[31] == 1)
                {
                    blink = true;
                    Debug.Log("1");
                    call = false;
                    if(!check)
                        break;
                }
                else if (receiveBytes[31] == 0)
                {
                    blink = false;
                    Debug.Log("0");
                    call = false;
                    if(!check)
                        break;
                }
                //Debug.Log($" {Encoding.Default.GetString(receiveBytes, 0, receiveBytes.Length)}");
                //Debug.Log(receiveBytes[31]);
                //Debug.Log(returnData);

            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
