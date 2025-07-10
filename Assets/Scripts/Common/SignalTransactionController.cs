using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalTransactionController : MonoBehaviour
{
    Vector3 _tranf_size_e = Vector3.one; // scale end
    Vector3 _tranf_size_s = Vector3.one; // scale start
    float _tranf_rotate_e = 0; // r end
    float _tranf_rotate_s = 0; // r start
    Vector3 _trans_pos = Vector3.zero;
    float total_distance_p = -1;
    float total_distance_s = -1;
    float total_distance_r = -1;
    float speed_s = 0f;
    float speed_p = 0f;
    float speed_r = 0f;


    public delegate void TransactionEnded();
    TransactionEnded _delegate;

    public delegate void OnTransactionPregress(float progress);
    OnTransactionPregress _delegate_ext;

    enum TransactType {
        None,
        Position,
        Scale,
        Rotate,
        ScalePos_mix,
        RotatePos_mix
    }

    public enum EaseType {
        None,
        EaseCubic,
        EaseOutCubic,
        EaseQuad,
        Bounce,
        BounceIn,
        
    }

    TransactType curType = TransactType.None;
    EaseType curEaseType = EaseType.None;
    List<float> midEvents = new();

    void Start(){}

    // Update is called once per frame
    void Update()
    {
        // Update position
        if ((curType == TransactType.ScalePos_mix || curType == TransactType.RotatePos_mix || curType == TransactType.Position) &&  speed_p != 0 )
        {
            // make easing transaction
            if (total_distance_p != -1)
            {
                
                float step_e = Vector3.Distance(transform.localPosition, _trans_pos);
                if (step_e > 1f)
                {
                    float p = (total_distance_p - step_e) / total_distance_p;
                    float e_progress = GetEasingChanges((p) + 0.15f) * (speed_p * Time.deltaTime);
                    var n_pos = Vector3.MoveTowards(transform.localPosition, _trans_pos, e_progress);
                    transform.localPosition = n_pos;
                    _delegate_ext?.Invoke(p);
                }
                else
                {
                    curType = TransactType.None;
                    speed_p = 0;
                    total_distance_p = -1;
                    transform.localPosition = _trans_pos;
                    _delegate?.Invoke();
                    _delegate_ext = null;
                }
            }
        }
        

        // Update scale
        if ((curType == TransactType.ScalePos_mix || curType == TransactType.Scale)  && speed_s != 0)
        {
            if (total_distance_s != -1)
            {
                total_distance_s += (speed_s * Time.deltaTime);

                float e_progress = GetEasingChanges(total_distance_s / 100);
                 
                var n_scale = new Vector3(
                    _tranf_size_s.x + ((_tranf_size_e.x - _tranf_size_s.x) * e_progress) ,
                    _tranf_size_s.y + ((_tranf_size_e.y - _tranf_size_s.y) * e_progress) ,
                    _tranf_size_s.z + ((_tranf_size_e.z - _tranf_size_s.z) * e_progress) );
                transform.localScale = n_scale;

                if (total_distance_s > 100)
                {
                    speed_s = 0;
                    total_distance_s = -1;
                    transform.localScale = _tranf_size_e;
                    
                    // invoke callback only scale
                    if (curType == TransactType.Scale) {
                        curType = TransactType.None;
                        _delegate?.Invoke();
                        //_delegate = null;
                    }   
                }
            }
        }

        // Update rotation
        if ((curType == TransactType.RotatePos_mix || curType == TransactType.Rotate) && speed_r != 0)
        {
            if (total_distance_r != -1)
            {
                total_distance_r += (speed_s * Time.deltaTime);

                float e_progress = GetEasingChanges(total_distance_r / 100);
                
                transform.localRotation = Quaternion.AngleAxis(_tranf_rotate_e - _tranf_rotate_s * e_progress, Vector3.forward);

                if (total_distance_r > 100)
                {
                    speed_r = 0;
                    total_distance_r = -1;
                    transform.localRotation = Quaternion.AngleAxis(_tranf_rotate_e, Vector3.forward);

                    // invoke callback only scale
                    if (curType == TransactType.Rotate)
                    {
                        curType = TransactType.None;
                        _delegate?.Invoke();
                        //_delegate = null;
                    }
                }
            }
        }

        UpdateTrack();
    }



    List<ItemTranscatPorperty> transact_track ;
    float transact_dur = 1f; // 1 sec
    float trackTimeCounting = 0;
    int lt_pos_idx = 0;
    int lt_scale_idx = 0;
    bool isTransactWithBaseScale = false;
    Vector3 base_scale = Vector3.one;
    Vector3 base_pos = Vector3.one;

    float mid_s_p = 0;
    void UpdateTrack()
    {
        if (gameObject.activeSelf && transact_track != null && transact_dur > 0)
        {
            trackTimeCounting += Time.deltaTime;

            if (transact_track.Count > 1 && trackTimeCounting < transact_dur)
            {
                float on_p = (trackTimeCounting % transact_dur);
                //position
                {
                    // change the start end
                    if (transact_track.Count > 2)
                    {
                        foreach (var tMark in transact_track)
                        {
                            int idx = transact_track.IndexOf(tMark);
                            if (idx > 0)
                            {
                                if (trackTimeCounting > (tMark.timeMark / 100f) * transact_dur && idx + 1 < transact_track.Count)
                                {
                                    if(idx > lt_pos_idx)
                                        lt_pos_idx = idx;
                                }
                            }
                        }
                    }

                    // recalu the dur in between 2 track
                    float mid_d = (transact_track[lt_pos_idx + 1].timeMark - transact_track[lt_pos_idx].timeMark) / 100f * transact_dur;
                    Vector3 _tranf_pos_s = transact_track[lt_pos_idx].position;
                    Vector3 _tranf_pos_e = transact_track[lt_pos_idx + 1].position;
                    
                    /// make transcation
                    float e_progress = GetEasingChanges((trackTimeCounting - (transact_track[lt_pos_idx].timeMark / 100f * transact_dur)) / mid_d);
                    if (e_progress > 0f) {
                        var n_pos = new Vector3(
                            _tranf_pos_s.x + ((_tranf_pos_e.x - _tranf_pos_s.x) * e_progress),
                            _tranf_pos_s.y + ((_tranf_pos_e.y - _tranf_pos_s.y) * e_progress),
                            _tranf_pos_s.z + ((_tranf_pos_e.z - _tranf_pos_s.z) * e_progress));
                        transform.localPosition = n_pos;
                    }
                    
                }

                //scale
                {
                    // change the start end
                    
                    if (transact_track.Count > 2)
                    {
                        foreach (var tMark in transact_track)
                        {
                            int idx = transact_track.IndexOf(tMark);

                            if (idx > 0)
                            {
                                if (trackTimeCounting > (tMark.timeMark / 100f) * transact_dur && idx + 1 < transact_track.Count)
                                {
                                    if (idx > lt_scale_idx)
                                        lt_scale_idx = idx;
                                }
                            }
                        }
                    }

                    // recalu the dur in between 2 track
                    float mid_d_s = (transact_track[lt_scale_idx + 1].timeMark - transact_track[lt_scale_idx].timeMark) / 100f * transact_dur;
                    Vector3 _tranf_size_s = transact_track[lt_scale_idx].scale;
                    Vector3 _tranf_size_e = transact_track[lt_scale_idx + 1].scale;
                    if (isTransactWithBaseScale) {
                        _tranf_size_s = base_scale + transact_track[lt_scale_idx].scale;
                        _tranf_size_e = base_scale + transact_track[lt_scale_idx + 1].scale;
                    }

                    /// make transcation
                    float e_progress = ((trackTimeCounting - (transact_track[lt_scale_idx].timeMark / 100f * transact_dur)) / mid_d_s);
                    //float e_progress = GetEasingChanges((trackTimeCounting - (transact_track[lt_scale_idx].timeMark / 100f * transact_dur)) / mid_d_s);
                    //e_progress = EasingCal.EaseInCubic(1 * on_p / (transact_dur * 100));
                    if (e_progress > 0f)
                    {
                        var n_scale = new Vector3(
                            _tranf_size_s.x + ((_tranf_size_e.x - _tranf_size_s.x) * e_progress),
                            _tranf_size_s.y + ((_tranf_size_e.y - _tranf_size_s.y) * e_progress),
                            _tranf_size_s.z + ((_tranf_size_e.z - _tranf_size_s.z) * e_progress));
                        transform.localScale = n_scale;
                    }
                        
                }

                //color
                {
                    // change the start end
                    if (transact_track.Count > 2)
                    {
                        foreach (var tMark in transact_track)
                        {
                            int idx = transact_track.IndexOf(tMark);
                            if (idx > 0)
                            {
                                if (trackTimeCounting > (tMark.timeMark / 100f) * transact_dur && idx + 1 < transact_track.Count)
                                {
                                    if (idx > lt_pos_idx)
                                        lt_pos_idx = idx;
                                }
                            }
                        }
                    }

                    // recalu the dur in between 2 track
                    float mid_d = (transact_track[lt_pos_idx + 1].timeMark - transact_track[lt_pos_idx].timeMark) / 100f * transact_dur;
                    Color _tranf_c_s = transact_track[lt_pos_idx].color;
                    Color _tranf_c_e = transact_track[lt_pos_idx + 1].color;

                    /// make transcation
                    float e_progress = GetEasingChanges((trackTimeCounting - (transact_track[lt_pos_idx].timeMark / 100f * transact_dur)) / mid_d);
                    if (e_progress > 0f)
                    {
                        var n_c = new Color(
                            _tranf_c_s.r + ((_tranf_c_e.r - _tranf_c_s.r) * e_progress),
                            _tranf_c_s.g + ((_tranf_c_e.g - _tranf_c_s.g) * e_progress),
                            _tranf_c_s.b + ((_tranf_c_e.b - _tranf_c_s.b) * e_progress),
                            _tranf_c_s.a + ((_tranf_c_e.a - _tranf_c_s.a) * e_progress));

                        //apply to All layer
                        {
                            // just image
                            foreach (var imageLayer in allImageLayer)
                            {
                                if (imageLayer != null)
                                    imageLayer.color = n_c;
                            }

                            // do change text
                            foreach (var textLayer in allTextLayer)
                            {
                                if (textLayer != null)
                                {
                                    var curColor = textLayer.color; // keep the current base colour
                                    textLayer.color = new Color(curColor.r, curColor.g, curColor.b, n_c.a);
                                }
                            }
                        }
                    }

                }

            }
            else {
                curType = TransactType.None;
                _delegate?.Invoke();
                lt_scale_idx = 0;
                lt_pos_idx = 0;
                trackTimeCounting = 0;
                transact_dur = -1;
            }
        }
    }

    float GetEasingChanges(float progress) {

        float _progress = progress;
        switch (curEaseType)
        {
            case EaseType.None:
            default:
                break;
            case EaseType.EaseCubic:
                _progress = EasingCal.EaseInCubic(1 * progress );
                break;
            case EaseType.EaseQuad:
                _progress = EasingCal.EaseInOutQuad(1 * progress );
                break;
            case EaseType.Bounce:
                _progress = EasingCal.BounceOut(1 * progress );
                break;
            case EaseType.BounceIn:
                _progress = EasingCal.BounceIn(1 * progress );
                break;
            case EaseType.EaseOutCubic:
                _progress = EasingCal.EaseOutCubic(1 * progress );
                break;
        }

        return _progress;
    }

    /// =-=-=-=-=-=-=- Events =-=--=-=-=--=-=-=-

    public void SetProgressListener( OnTransactionPregress n_listener ) {
        if (_delegate_ext != null)
        {
            var cur_deles = new List<Delegate>(_delegate_ext.GetInvocationList());
            if (!cur_deles.Contains(n_listener))
                _delegate_ext += n_listener;
        }
        else {
            _delegate_ext += n_listener;
        }
    }

    /// =-=-=-=-=-=-=- positioning 
    public void CastTransactionStartWithCurrentPos(Vector3 end_pos, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        if (curType != TransactType.ScalePos_mix && curType != TransactType.RotatePos_mix)
        {
            curType = TransactType.Position;
            _delegate = n_callback;
        }
        
        _trans_pos = end_pos;
        curEaseType = easeReq;
        total_distance_p = Vector3.Distance(transform.localPosition, end_pos);
        speed_p = transaction_speed;
    }

    public void CastTransaction(Vector3 start_pos , Vector3 end_pos , float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null) {
        transform.localPosition = start_pos;
        CastTransactionStartWithCurrentPos(end_pos , transaction_speed, easeReq, n_callback);
    }


    /// =-=-=-=-=-=-=- scaling =-=--=-=-=--=-=-=-

    public void CastScaleStartWithCurrentSize(Vector3 end_scale, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        
        if (curType != TransactType.ScalePos_mix) {
            curType = TransactType.Scale;
            _delegate = n_callback;
        }
        curEaseType = easeReq;
        _tranf_size_e = end_scale;
        total_distance_s = 0;
        /// TODO : make transcations
        speed_s = transaction_speed;
    }

    public void CastScale(Vector3 start_scale, Vector3 end_scale, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        transform.localScale = start_scale;
        _tranf_size_s = start_scale;
        CastScaleStartWithCurrentSize(end_scale, transaction_speed, easeReq, n_callback);
    }

    /// =-=-=-=-=-=-=- rotation =-=--=-=-=--=-=-=-

    public void CastRotateStartWithCurrentSize(float end_angle, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        if (curType != TransactType.RotatePos_mix)
        {
            curType = TransactType.Rotate;
            _delegate = n_callback;
        }
        curEaseType = easeReq;
        _tranf_rotate_e = end_angle;
        total_distance_r = 0;
        /// TODO : make transcations
        speed_r = transaction_speed;
    }

    public void CastRotate(float start_angle, float end_angle, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        transform.localRotation  = Quaternion.AngleAxis(start_angle, Vector3.forward); 
        _tranf_rotate_s = start_angle;
        CastRotateStartWithCurrentSize(end_angle, transaction_speed, easeReq, n_callback);
    }

    /// =-=-=-=-=-=-=- Mix =-=--=-=-=--=-=-=-

    public void CastScaleAndMove(Vector3 start_pos, Vector3 end_pos, Vector3 start_scale, Vector3 end_scale,  float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        curType = TransactType.ScalePos_mix;
        _delegate = n_callback;
        /// make both transcations
        CastTransaction(start_pos, end_pos , transaction_speed , easeReq , null);
        CastScale(start_scale, end_scale, transaction_speed);
    }

    public void CastRotateAndMove(Vector3 start_pos, Vector3 end_pos, float start_angle, float end_angle, float transaction_speed, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        curType = TransactType.RotatePos_mix;
        _delegate = n_callback;
        /// make both transcations
        CastTransaction(start_pos, end_pos, transaction_speed, easeReq, null);
        CastRotate(start_angle, end_angle, transaction_speed);
    }

    /// <summary>
    /// This cast will follow the list of ItemTranscatPorperty within the dur to present the hole transactions
    /// </summary>
    /// <param name="n_track"></param>
    /// <param name="dur"></param>
    /// <param name="transaction_speed"></param>
    /// <param name="easeReq"></param>
    /// <param name="n_callback"></param>
    public void CastTrackTransaction(List<ItemTranscatPorperty> n_track, float dur , EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        _delegate = n_callback;
        transact_track = n_track;
        transact_dur = dur;
        base_scale = n_track[0].scale;
        base_pos = n_track[0].position;
        curEaseType = easeReq;
        GetAllChildLayer(gameObject.transform);
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void CastTrackTransactionWithBaseScale(List<ItemTranscatPorperty> n_track, float dur, EaseType easeReq = EaseType.None, TransactionEnded n_callback = null)
    {
        CastTrackTransaction(n_track, dur, easeReq, n_callback);
        isTransactWithBaseScale = true;
        base_scale = transform.localScale;
        base_pos = transform.localPosition;
    }

    List<Image> allImageLayer = null;
    List<Text> allTextLayer = null;
    Image[] imagesIgnore = null;
    void GetAllChildLayer(Transform targetLayer)
    {
        var selfImageLayer = targetLayer.GetComponent<Image>();
        if (selfImageLayer != null) {
            if (allImageLayer == null)
            {
                allImageLayer = new();
                allTextLayer = new();
            }

            allImageLayer.Add(selfImageLayer);
        }

        if (targetLayer.childCount > 0)
        {
            if (allImageLayer == null)
            {
                allImageLayer = new();
                allTextLayer = new();
            }

            var layers = targetLayer.GetComponentsInChildren<Image>(false);
            allImageLayer.AddRange(layers);
            var layers_text = targetLayer.GetComponentsInChildren<Text>(false);
            allTextLayer.AddRange(layers_text);


            // do remove 
            if (imagesIgnore != null) {
                foreach (var filterOut in imagesIgnore)
                {
                    if (allImageLayer.Contains(filterOut))
                    {
                        allImageLayer.Remove(filterOut);
                    }
                }
            }

        }
    }

}

public class ItemTranscatPorperty
{
    public int timeMark = 0;
    public Vector3 scale = Vector3.one;
    public Vector3 position = Vector3.zero;
    public Color color = Color.white;
    public Quaternion quaternion = Quaternion.AngleAxis(0, Vector3.forward);

    public ItemTranscatPorperty(int n_timeMark, Vector3 n_scale)
    {
        timeMark = n_timeMark;
        scale = n_scale;
    }

    public ItemTranscatPorperty(int n_timeMark, Vector3 n_position, Vector3 n_scale)
    {
        timeMark = n_timeMark;
        position = n_position;
        scale = n_scale;
    }

    public ItemTranscatPorperty(int n_timeMark, Vector3 n_position, Vector3 n_scale , Color n_color)
    {
        timeMark = n_timeMark;
        position = n_position;
        scale = n_scale;
        color = n_color;
    }
}
