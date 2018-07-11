using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUtil
{

	public static float MoveFrame(CharacterController characterController, Transform target,float MoveSpeed, float TurnSpeed)
    {
        Transform T = characterController.transform;
        Vector3 Dir = target.position - T.position;
        Vector3 DirXZ = new Vector3(Dir.x, 0, Dir.z);
        Vector3 TargetPos = T.position + DirXZ;
        Vector3 framePos = Vector3.MoveTowards(T.position, TargetPos, MoveSpeed * Time.deltaTime);

        characterController.Move(framePos-T.position+Physics.gravity);

        RotateToDir(T, target, TurnSpeed);


        return Vector3.Distance(framePos, TargetPos);
    }

    public static float MoveFrame(CharacterController characterController, Vector3 target, float MoveSpeed, float TurnSpeed)
    {
        Transform T = characterController.transform;
        Vector3 Dir = target - T.position;
        Vector3 DirXZ = new Vector3(Dir.x, 0, Dir.z);
        Vector3 TargetPos = T.position + DirXZ;
        Vector3 framePos = Vector3.MoveTowards(T.position, TargetPos, MoveSpeed * Time.deltaTime);

        characterController.Move(framePos - T.position + Physics.gravity);

        return Vector3.Distance(framePos, TargetPos);
    }

    public static void RotateToDir(Transform self, Transform target, float turnSpeed)
    {
        Vector3 Dir = target.position - self.position;
        Vector3 DirXZ = new Vector3(Dir.x, 0, Dir.z);

        if (DirXZ == Vector3.zero)
            return;


        self.rotation = Quaternion.RotateTowards(self.rotation, Quaternion.LookRotation(DirXZ), turnSpeed * Time.deltaTime);
        self.rotation = new Quaternion(self.rotation.x, self.rotation.y, self.rotation.z, self.rotation.w);
        
    }

    public static void RotateToDirBurst(Transform self, Transform target)
    {
        Vector3 Dir = Vector3.Normalize(target.position - self.position);
        Vector3 DirXZ = new Vector3(Dir.x, 0, Dir.z);

        if (DirXZ == Vector3.zero) return;

        self.rotation = Quaternion.LookRotation(DirXZ);
    }
}
