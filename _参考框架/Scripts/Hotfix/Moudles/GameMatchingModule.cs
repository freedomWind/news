// using System;
// using FrameOperateSync;
// using Game1.Core;
// using GameFramework;
// using GameFramework.Procedure;
// using UnityEngine;
// using UnityGameFramework.Runtime;
//
// namespace Game1
// {
//     public partial class GameMatchingModule : IGameModule,IFrameLoger, IFrameParser
//     {
//         private MatchingState _state;
//         private float _loginTime;
//         private float _matchingTime;
//         private static int _playerIdMy;
//         private IMatchingServer _server;
//
//         public MatchingState State => _state;
//         public float LoginTime => _loginTime;
//         public float MatchingTime => _matchingTime;
//         public static int MyPlayerId => _playerIdMy;
//         
//         public void InitServer(int type)
//         {
//             switch (type)
//             {
//                 case 0:  //单
//                     _server = gameObject.AddComponent<LocalServer>();
//                     break;
//                 case 1:   //
//                     _server = gameObject.AddComponent<BattleServer>();
//                     break;
//             }
//             if (_server == null)
//                 throw new Exception("matching server is null");
//          
//             _server.GameStartLoadEvt += OnGameLoad;
//             _server.GameBeginEvt += OnGameBegin;
//             _server.LoginResultEvt += OnLoginResult;
//             _server.MatchResultEvt += OnMatchResult;
//             _server.RoomInfoUpdateEvt += OnRoomInfoUpdate;
//             _server.PlayerLoadingEvt += OnPlayerLoadingUpdate;
//             _server.Init();
//             
//             ClientUtility.Init(this,this);
//             ClientUtility.StartFrame();
//             NetworkUtility.SetNetwork(_server as IFrameNetwork);
//         }
//         
//         public void Quit()
//         {
//             if (_server == null)
//                 return;
//             _server.Exit();
//         }
//
//         public void StartConnect()
//         {
//             if(_state != MatchingState.None)
//                 throw new Exception("当前状态不允许");
//             _state = MatchingState.StartConnectState;
//         }
//
//         public void StartMatch()
//         {
//             if (_state != MatchingState.ConnectedState)
//                 throw new Exception("当前状态不允许");
//             _state = MatchingState.StartMatchingState;
//         }
//
//         public void JoinMatch()
//         {
//             if (_state != MatchingState.MatchingSuccessState)
//                 throw new Exception("当前状态不允许");
//             _state = MatchingState.JoinBattleState;
//         }
//
//         private void Update()
//         {
//             if (_server == null)
//                 return;
//             
//             var fsm = EntityInst.Fsm.GetFsm<IProcedureManager>();
//             switch (_state)
//             {
//                 case MatchingState.StartConnectState: //connect
//                     _server.LoginServer("120.79.143.37",4098);
//                     _state = MatchingState.ConnectingState;
//                     _loginTime = 0;
//                     break;
//                 case MatchingState.ConnectingState:
//                     _loginTime += UnityEngine.Time.deltaTime;
//                     break;
//                 case MatchingState.ConnectedState:
//                     break;
//                 case MatchingState.StartMatchingState:
//                     _server.StartMatch();
//                     _state = MatchingState.MatchingState;
//                     _matchingTime = 0;
//                     break;
//                 case MatchingState.MatchingState: 
//                     _matchingTime += UnityEngine.Time.deltaTime;
//                     break;
//                 case MatchingState.MatchingSuccessState: //matching success
//                     break; 
//                 case MatchingState.MatchingFailureState:
//                     break;
//                 case MatchingState.JoinBattleState:
//                     _server.JoinMatch();
//                     _state = MatchingState.JoinBattleSuccessState;  //to fix
//                     break;
//                 case MatchingState.JoinBattleSuccessState:
//                     GameModuleComponent.MM.GetModule<GameBattleModule>().GameStartLoad();
//                     break;
//                 case MatchingState.JoinBattleFailureState:
//                     break;
//                 case MatchingState.LoadingBattleState:
//                     var loading = fsm.GetData<VarInt32>("BattleLoading").Value;
//                     Debug.Log($"上报游戏加载进度:{loading}");
//                     _server.PushLoadingProgress(loading);
//                     break;
//                 case MatchingState.StartBattleState:
//                     GameModuleComponent.MM.GetModule<GameBattleModule>().GameStart();
//                     _state = MatchingState.BattleUpdatingState;
//                     break;
//                 case MatchingState.BattleUpdatingState:
//                     break;
//                 default:
//                     break;
//             }
//         }
//     }
//
//     partial class GameMatchingModule
//     {
//         private void OnLoginResult(bool result)
//         {
//             if(_state != MatchingState.ConnectingState)
//                 throw new Exception($"状态错误:{_state}");
//             Debug.Log($"战斗服登录：{result}");
//             _state = MatchingState.ConnectedState;
//         }
//
//         private void OnGameBegin()
//         {
//             if(_state != MatchingState.LoadingBattleState)
//                 throw new Exception($"状态错误:{_state}");
//             _state = MatchingState.StartBattleState;
//         }
//
//         private void OnGameLoad()
//         {
//             if(_state != MatchingState.JoinBattleSuccessState)
//                 throw new Exception($"状态错误:{_state}");
//             _state = MatchingState.LoadingBattleState;
//         }
//
//         private void OnMatchResult(int myPlayerId)
//         {
//             if (_state != MatchingState.MatchingState)
//                 throw new Exception($"状态错误:{_state}");
//             _state = MatchingState.MatchingSuccessState;
//             _playerIdMy = myPlayerId;
//         }
//
//         private void OnRoomInfoUpdate(IMatchingServer.PlayerInfo[] infos)
//         {
//             var room = GameModuleComponent.MM.GetModule<GameRoomModule>();
//             room.RoomInfo.Players.Clear();
//             foreach (var item in infos)
//             {
//                 room.RoomInfo.Players.Add(new GameRoomModule.RoomData.PlayerInfo()
//                 {
//                     playerId = item.playerId,
//                     side = BattleTypeUtility.GetSide(item.playerId),
//                     isReady = item.agree,
//                     loadingProgress = 0,
//                 });
//             }
//         }
//
//         private void OnPlayerLoadingUpdate(IMatchingServer.PlayerLoading[] loadings)
//         {
//             var room = GameModuleComponent.MM.GetModule<GameRoomModule>();
//             foreach (var loading in loadings)
//             {
//                 var x = room.RoomInfo.Players.Find(p => p.playerId == loading.player);
//                 x.loadingProgress = loading.loading;
//             }
//         }
//     }
//
//     partial class GameMatchingModule
//     {
//         void IFrameLoger.LogInfo(string msg)
//         {
//             // Log.Info(msg);
//             Debug.Log($"FrameInfo:{msg}");
//         }
//
//         void IFrameLoger.LogWarning(string msg)
//         {
//             Debug.LogWarning($"FrameInfo:{msg}");
//         }
//
//         void IFrameLoger.LogError(string msg)
//         {
//             Debug.LogError($"FrameInfo:{msg}");
//         }
//
//         string IFrameParser.ToJson(object data)
//         {
//             return UnityEngine.JsonUtility.ToJson(data);
//         }
//
//         object IFrameParser.FromJson(string json, Type type)
//         {
//             return UnityEngine.JsonUtility.FromJson(json, type);
//         }
//     }
//
//     partial class GameMatchingModule
//     {
//         public enum MatchingState : int
//         {
//             None,
//             StartConnectState,
//             ConnectingState,
//             ConnectedState,
//             StartMatchingState,
//             MatchingState,
//             MatchingSuccessState,
//             MatchingFailureState,
//             JoinBattleState,
//             JoinBattleFailureState,
//             JoinBattleSuccessState,
//             LoadingBattleState,
//             StartBattleState,
//             BattleUpdatingState,
//         }
//     }
// }