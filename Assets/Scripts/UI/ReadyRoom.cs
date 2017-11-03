﻿using Framework.UI;
using System.Collections.Generic;
using UnityEngine;
using Framework.Message;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using System.Collections;

namespace AGrail
{
    public class ReadyRoom : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private List<ReadyRoomPlayer> players = new List<ReadyRoomPlayer>();
        [SerializeField]
        private Text roomTitle;
        [SerializeField]
        private GameObject chooseTeamPanel;
        [SerializeField]
        private Button btnChooseRedTeam;
        [SerializeField]
        private Button btnChooseBlueTeam;
        [SerializeField]
        private Button btnChooseRandomTeam;
        [SerializeField]
        private Button ChooseTeam;
        [SerializeField]
        private Button btnExit;
        [SerializeField]
        private GameObject leaderPanel;
        [SerializeField]
        private GameObject leaderField;
        [SerializeField]
        private Button btnBecomeLeader;
        [SerializeField]
        private Button btnNotLeader;
        [SerializeField]
        private Text talks;

        public override WindowType Type
        {
            get
            {
                return WindowType.ReadyRoom;
            }
        }

        public override void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.RoomIDChange, this);
            MessageSystem<MessageType>.Regist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.Regist(MessageType.ChooseRole, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerLeave, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.Regist(MessageType.LEADERREQUEST, this);
            MessageSystem<MessageType>.Regist(MessageType.GameStart, this);
            MessageSystem<MessageType>.Regist(MessageType.ERROR, this);

            foreach (var v in players)
                v.Reset();
            roomTitle.text = string.Format("{0} {1}", Lobby.Instance.SelectRoom.room_id, Lobby.Instance.SelectRoom.room_name);
            if (Lobby.Instance.SelectRoom.max_player == 4)
            {
                GameObject.Destroy(players[5].gameObject);
                GameObject.Destroy(players[4].gameObject);
                players.RemoveRange(4, 2);
            }
            for(int i = 0; i < BattleData.Instance.PlayerInfos.Count; i++)
            {
                MessageSystem<MessageType>.Notify(MessageType.PlayerIsReady, i, BattleData.Instance.PlayerInfos[i].ready);
                MessageSystem<MessageType>.Notify(MessageType.PlayerNickName, i, BattleData.Instance.PlayerInfos[i].nickname);
                MessageSystem<MessageType>.Notify(MessageType.PlayerTeamChange, i, BattleData.Instance.PlayerInfos[i].team);
            }

            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1).OnComplete(() => { btnExit.interactable = true; }) ;

            btnChooseRedTeam.onClick.AddListener(delegate { chooseTeam(Team.Red); });
            btnChooseBlueTeam.onClick.AddListener(delegate { chooseTeam(Team.Blue); });
            btnChooseRandomTeam.onClick.AddListener(delegate { chooseTeam(Team.Other); });
            btnBecomeLeader.onClick.AddListener(delegate { chooseLeader(true); });
            btnNotLeader.onClick.AddListener(delegate { chooseLeader(false); });
            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.RoomIDChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ChooseRole, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerLeave, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.LEADERREQUEST, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GameStart, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ERROR, this);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.RoomIDChange:
                    roomTitle.text = string.Format("{0} {1}", Lobby.Instance.SelectRoom.room_id, Lobby.Instance.SelectRoom.room_name);
                    break;
                case MessageType.ChooseRole:
                    var roleStrategy = (network.ROLE_STRATEGY)parameters[0];
                    switch (roleStrategy)
                    {
                        case network.ROLE_STRATEGY.ROLE_STRATEGY_31:
                            if (RoleChoose.Instance.RoleIDs.Count > 3)
                                GameManager.UIInstance.PushWindow(WindowType.RoleChooseAny, WinMsg.Pause);
                            else
                                GameManager.UIInstance.PushWindow(WindowType.RoleChoose31, WinMsg.Pause);
                            break;
                        case network.ROLE_STRATEGY.ROLE_STRATEGY_BP:
                            if(GameManager.UIInstance.PeekWindow()!=WindowType.RoleChooseBPCM)
                            {
                                GameManager.UIInstance.PushWindow(WindowType.RoleChooseBPCM, WinMsg.Pause);
                            }
                            MessageSystem<MessageType>.Notify(MessageType.PICKBAN, this);
                            break;
                        case network.ROLE_STRATEGY.ROLE_STRATEGY_CM:
                            if (GameManager.UIInstance.PeekWindow() != WindowType.RoleChooseBPCM)
                                GameManager.UIInstance.PushWindow(WindowType.RoleChooseBPCM, WinMsg.Pause);
                            MessageSystem<MessageType>.Notify(MessageType.PICKBAN, this);
                            break;
                        default:
                            Debug.LogError("不支持的选将模式");
                            break;
                    }
                    break;
                case MessageType.PlayerTeamChange:
                    players[(int)parameters[0]].Team = (Team)(uint)parameters[1];
                    break;
                case MessageType.PlayerNickName:
                    players[(int)parameters[0]].PlayerName = parameters[1].ToString();
                    break;
                case MessageType.PlayerIsReady:
                    players[(int)parameters[0]].IsReady = (bool)parameters[1];
                    break;
                case MessageType.PlayerLeave:
                    players[(int)parameters[0]].Reset();
                    break;
                case MessageType.LEADERREQUEST:
                    leaderPanel.SetActive(true);
                    break;
                case MessageType.GameStart:
                    PlayerPrefs.SetInt("lastGame", Lobby.Instance.SelectRoom.room_id);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                    break;
                case MessageType.ChatChange:
                    var chat = Dialog.Instance.Chat.Last();
                    talks.text += (chat.RoleID == null) ? BattleData.Instance.GetPlayerInfo(chat.ID).nickname + ": " + chat.msg : chat.msg;
                    talks.text += "\n";
                    break;
                case MessageType.ERROR:
                    var errorProto = parameters[0] as network.Error;
                    Debug.LogError(errorProto.id);
                    if (errorProto.id == 32)
                    {
                        StartCoroutine(Wait1());
                    }
                    break;
            }
        }
        IEnumerator Wait1()
        {
            yield return new WaitForSeconds(1);
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InputBox, Framework.UI.WinMsg.None, -1, Vector3.zero,
                new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None); OnExitClick(); }),
                new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None); OnExitClick(); }),
                "瞎蒙果然是不行的~");
        }

        public void OnReadyClick()
        {
            ChooseTeam.interactable = BattleData.Instance.MainPlayer.ready;
            btnExit.interactable = BattleData.Instance.MainPlayer.ready;
            BattleData.Instance.Ready(!BattleData.Instance.MainPlayer.ready);
        }

        public void OnExitClick()
        {
            Lobby.Instance.LeaveRoom();
            Lobby.Instance.GetRoomList();
            GameManager.UIInstance.PopWindow(WinMsg.Resume);
        }

        public void OnBtnChooseTeamClick()
        {
            chooseTeamPanel.SetActive(true);
        }

        private void chooseTeam(Team team)
        {
            BattleData.Instance.ChooseTeam(team);
            chooseTeamPanel.SetActive(false);
        }

        private void chooseLeader(bool lead)
        {
            BattleData.Instance.ChooseLeader(lead);
            leaderField.SetActive(false);
        }
    }
}


